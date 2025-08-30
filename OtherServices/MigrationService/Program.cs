using CommandLine;
using Legal.Application.Admin;
using Legal.MigrationService;
using Legal.MigrationService.ContextFactories;
using Legal.Service.Infrastructure.Model;
using System.Reflection;

internal class Program
{
    /*
     * Migration service is a command line tool to manage MNE migrations.
     * By default, appsetings.json has the connection strings.
     *
     * Adding a migration:
     * Make sure you have "MigrationService" selected as default project
     * Then run migration command: add-migration Test_Migration -Context CommonContext
     *
     * Basic commands:
     * --auth-db "[connection-string]": To override appsettings StreamLoginConnection
     * For example, MigrationService.exe --auth-db "connection_string"
     *
     * --data-db "[connection-string]": To override appsettings StreamDataConnection
     * For example, MigrationService.exe --data-db "connection_string"
     *
     * --update-database "[y|n]": Input "y" to start applying migrations
     * For example, MigrationService.exe --update-database "y"
     *
     * If no commands are given then a prompt will appear asking whether to start the migration or not.
     *
     * mnetools.exe:
     * Command line tool for creating blank sql scripts. To create blank sql script navigate to the MigrationService
     * project folder and run this command:
     * For example, mnetools.exe --create "seed_users" --db "data"
     *
     * --create "[script-name]": To generate SQL script
     * --db "[database-name(auth|data)]": Targeting database for the blank sql file
     *
     * */

    private static void Main(string[] args)
    {
        // Diagnostics
        Console.WriteLine("Raw args length: " + args.Length);
        Console.WriteLine("Args[]: '" + string.Join("' '", args) + "'");
        var allCmd = Environment.GetCommandLineArgs();
        Console.WriteLine("Environment.GetCommandLineArgs(): '" + string.Join("' '", allCmd) + "'");
        Console.WriteLine("Process command line: " + Environment.CommandLine);

        // Auto fallback: if running under VS/containers with no args but env vars present
        if (args.Length == 0)
        {
            var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");
            var autoRun = Environment.GetEnvironmentVariable("MIGRATION_AUTO_RUN");
            if (!string.IsNullOrWhiteSpace(envConn))
            {
                Console.WriteLine("No CLI args supplied. Using environment fallback (ConnectionStrings__Postgres).");
                var fallback = new Options
                {
                    StreamDataConnection = envConn.Trim(),
                    UpdateDatabase = string.IsNullOrWhiteSpace(autoRun) ? "n" : "y",
                    ContextNumber = "-1"
                };
                RunOptions(fallback);
                return; // skip CommandLine parser
            }
        }

        // Attempt to recover inline args if a single command line contains them
        if (args.Length == 0 && Environment.CommandLine.Contains("--data-db"))
        {
            args = SplitArgs(Environment.CommandLine)
                .Where(a => a.StartsWith("--"))
                .ToArray();
            Console.WriteLine("Recovered args: '" + string.Join("' '", args) + "'");
        }

        MappingConfig.RegisterMappings();
        Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
    }

    // Minimal arg splitter (does not handle escaped quotes inside quoted values)
    private static IEnumerable<string> SplitArgs(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine)) yield break;
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        // skip the first token (host executable path)
        bool firstTokenSkipped = false;
        foreach (var ch in commandLine)
        {
            if (!firstTokenSkipped)
            {
                if (ch == ' ') { firstTokenSkipped = true; }
                continue;
            }
            if (ch == '"') { inQuotes = !inQuotes; continue; }
            if (!inQuotes && char.IsWhiteSpace(ch))
            {
                if (current.Length > 0)
                {
                    yield return current.ToString();
                    current.Clear();
                }
            }
            else
            {
                current.Append(ch);
            }
        }
        if (current.Length > 0)
            yield return current.ToString();
    }

    private static void RunOptions(Options opts)
    {
        var configuration = new Configuration(opts);
        var aDatabaseContexts = GetTypes();
        Console.WriteLine($"Current database: \n{configuration.ConString}");
        Console.WriteLine("Contexts found: ");
        int index = 1;
        foreach (var context in aDatabaseContexts)
        {
            Console.WriteLine($"{index}. {context?.Name}");
            index++;
        }

        Console.WriteLine(Environment.NewLine);
        string? selectedCxtId = "-1";
        string? ans = "n";
        if (!string.IsNullOrEmpty(opts.ContextNumber))
        {
            selectedCxtId = opts.ContextNumber;
        }
        else
        {
            Console.WriteLine("Please select your context (number)");
            Console.Write("=> ");
            selectedCxtId = Console.ReadLine()?.ToLower();
        }

        if (!string.IsNullOrEmpty(opts.UpdateDatabase) && opts.UpdateDatabase == "y")
        {
            ans = "y";
        }
        else
        {
            Console.WriteLine("This operation is going to update your database. Are you sure you want to do this? (y/n)");
            Console.Write("=> ");
            ans = Console.ReadLine()?.ToLower();
        }

        if (!string.IsNullOrEmpty(ans) && ans.Equals("y") && !string.IsNullOrEmpty(selectedCxtId))
        {
            if (selectedCxtId.Equals("-1"))
            {
                foreach (var contextType in aDatabaseContexts.Where(t => t != null))
                {
                    InvokeMigrateDb(configuration, contextType!);
                }
                return;
            }
            else
            {
                var cxtIds = selectedCxtId.Split(',');
                if (cxtIds.Length >= 1)
                {
                    foreach (var cxtId in cxtIds)
                    {
                        var id = int.Parse(cxtId) - 1;
                        var contextType = aDatabaseContexts[id];
                        InvokeMigrateDb(configuration, contextType);
                    }
                    return;
                }
                var cxtRange = selectedCxtId.Split('-');
                if (cxtRange.Length > 1)
                {
                    foreach (var item in Enumerable.Range(int.Parse(cxtRange[0]), int.Parse(cxtRange[1])))
                    {
                        var contextType = aDatabaseContexts[item - 1];
                        InvokeMigrateDb(configuration, contextType);
                    }
                    return;
                }
            }
        }
    }

    private static List<Type> GetTypes()
    {
        var types = new List<Type>();
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        AssemblyName[] referencedAssemblies = executingAssembly.GetReferencedAssemblies();
        foreach (AssemblyName assemblyName in referencedAssemblies.Where(q => q.Name.Contains("Legal.Application.")))
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                Console.WriteLine($"Assembly: {assembly.FullName}");
                types.Add(assembly.GetTypes().FirstOrDefault(q => q.BaseType?.Name == nameof(ADatabaseContext))!);
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Error loading types from assembly: {assemblyName.FullName}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine($"  Loader Exception: {loaderException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error loading assembly: {assemblyName.FullName}");
                Console.WriteLine($"  Exception: {ex.Message}");
            }
        }
        return types.Where(t => t != null).ToList();
    }

    private static void InvokeMigrateDb(Configuration configuration, Type type)
    {
        Type cxtFactoryGeneric = typeof(DbContextFactory<>);
        Type cxtFactoryGenericClass = cxtFactoryGeneric.MakeGenericType(type);
        object cxtFactory = Activator.CreateInstance(cxtFactoryGenericClass, configuration)!;
        Type myGeneric = typeof(Migrator<>);
        Type constructedClass = myGeneric.MakeGenericType(type);
        object created = Activator.CreateInstance(constructedClass, configuration, cxtFactory)!;
        var method = constructedClass.GetMethod("MigrateDb");
        _ = method!.Invoke(created, Array.Empty<object>());
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var item in errs)
        {
            Console.WriteLine(item.ToString());
        }
    }
}
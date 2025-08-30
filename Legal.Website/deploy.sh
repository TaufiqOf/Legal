```bash
#!/bin/bash

# Legal Website Deployment Script
# This script builds the Angular application for production

echo "Starting Legal Website deployment..."

# Install dependencies
echo "Installing dependencies..."
npm install

# Build for production
echo "Building application for production..."
npm run build

# Check if build was successful
if [ $? -eq 0 ]; then
    echo "Build completed successfully!"
    echo "Build artifacts are available in the dist/ directory"
    echo ""
    echo "To serve the application:"
    echo "1. Copy the contents of dist/ to your web server"
    echo "2. Configure your web server to serve index.html for all routes"
    echo "3. Ensure the API backend is running and accessible"
    echo ""
    echo "For development:"
    echo "npm start - Start development server"
    echo "npm run watch - Build and watch for changes"
else
    echo "Build failed! Please check the errors above."
    exit 1
fi
```

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ContractListComponent } from './components/contract-list/contract-list.component';
import { ContractFormComponent } from './components/contract-form/contract-form.component';
import { ContractViewComponent } from './components/contract-view/contract-view.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/contracts', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'contracts', component: ContractListComponent, canActivate: [AuthGuard] },
  { path: 'contracts/create', component: ContractFormComponent, canActivate: [AuthGuard] },
  { path: 'contracts/edit/:id', component: ContractFormComponent, canActivate: [AuthGuard] },
  { path: 'contracts/view/:id', component: ContractViewComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/contracts' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

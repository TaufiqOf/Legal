import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ContractListComponent } from './components/contract-list/contract-list.component';
import { ContractFormComponent } from './components/contract-form/contract-form.component';
import { ContractViewComponent } from './components/contract-view/contract-view.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { PaginationComponent } from './shared/pagination/pagination.component';

@NgModule({
  declarations: [
    App,
    LoginComponent,
    RegisterComponent,
    ContractListComponent,
    ContractFormComponent,
    ContractViewComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule,
    PaginationComponent
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [App]
})
export class AppModule { }

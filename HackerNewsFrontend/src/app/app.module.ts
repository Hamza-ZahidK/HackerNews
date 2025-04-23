import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';  
import { TableModule } from 'primeng/table'; 
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms'; 
import { ProgressSpinnerModule } from 'primeng/progressspinner'; 
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule  ,
    TableModule, 
    ButtonModule,
    FormsModule,
    ProgressSpinnerModule
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]  
})
export class AppModule { }

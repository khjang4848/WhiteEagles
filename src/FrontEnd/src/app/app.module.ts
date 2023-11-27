import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { AppComponent } from "./app.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HeaderComponent } from "./components/header/header.component";
import {AppRoutingModule} from './app.routing.module';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {ConfigService} from './service/config/config.service';
import {WalletService} from './service/wallet.service';
import {MerchantService} from './service/merchant.service';
import {NgxSpinnerModule} from 'ngx-spinner';
import {FormsModule} from '@angular/forms';
import {AppMaterialModule} from "./app.material.module";
import {HttpInterceptorService} from "./service/config/http.interceptor.service";
import {UserService} from "./service/user.service";
import {CalculateService} from "./service/calculate.service";
import {CurrencyMaskInputMode, NgxCurrencyModule} from "ngx-currency";

export const customCurrencyMaskConfig = {
    align: "right",
    allowNegative: true,
    allowZero: true,
    decimal: ",",
    precision: 0,
    prefix: "\\",
    suffix: "",
    thousands: ",",
    nullable: true,
    min: null,
    max: null,
    inputMode: CurrencyMaskInputMode.FINANCIAL
};
@NgModule({
    declarations: [
        AppComponent,
        HeaderComponent
    ],
    imports: [
        NgxCurrencyModule.forRoot(customCurrencyMaskConfig),
        AppMaterialModule, AppRoutingModule,
      BrowserModule,
      NgxDatatableModule, HttpClientModule, NgxSpinnerModule, BrowserAnimationsModule,
      FormsModule
    ],
    providers: [{
        provide: HTTP_INTERCEPTORS,
        useClass: HttpInterceptorService,
        multi: true
    }, ConfigService, MerchantService, WalletService, UserService, CalculateService],
    bootstrap: [AppComponent]
})
export class AppModule { }

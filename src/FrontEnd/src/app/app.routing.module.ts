import {RouterModule, Routes} from '@angular/router';
import {ConfigurationComponent} from './components/configuration/configuration.component';
import {NgModule} from '@angular/core';
import {WallethistoryComponent} from './components/wallethistory/wallethistory.component';
import {AppMaterialModule} from "./app.material.module";
import {NgxDatatableModule} from "@swimlane/ngx-datatable";
import {FormsModule} from "@angular/forms";
import {LimitlistComponent} from "./components/limit/limitlist/limitlist.component";
import { LimitregisterComponent } from './components/limit/limitregister/limitregister.component';
import { TransferComponent } from './components/transfer/transfer.component';
import { CalculatelistComponent } from './components/calculation/calculatelist/calculatelist.component';
import { CalculatereportComponent } from './components/calculation/calculatereport/calculatereport.component';
import {NgxCurrencyModule} from "ngx-currency";

const appRoutes: Routes = [
    {path: "", component: ConfigurationComponent},
    {path: "walletHistory", component: WallethistoryComponent},
    {path: "limitList", component: LimitlistComponent},
    {path: "limitRegister/:mid", component: LimitregisterComponent},
    {path: "transfer/:mid", component: TransferComponent},
    {path: "calculate", component: CalculatelistComponent},
    {path: "calculatereport/:mid", component: CalculatereportComponent},
];
@NgModule({
    imports: [RouterModule.forRoot(appRoutes), AppMaterialModule, NgxDatatableModule, FormsModule, NgxCurrencyModule],
    exports: [RouterModule],
    declarations: [ConfigurationComponent, WallethistoryComponent, LimitlistComponent, LimitregisterComponent,
        TransferComponent, CalculatelistComponent, CalculatereportComponent]
})
export class AppRoutingModule {}

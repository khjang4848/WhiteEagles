import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {WalletService} from "../../../service/wallet.service";
import {MerchantService} from "../../../service/merchant.service";
import {LimitRegisterViewModel} from "../../../models/LimitRegisterViewModel";
import {LimitRegister} from "../../../models/LimitRegister";

@Component({
    selector: 'app-limitregister',
    templateUrl: './limitregister.component.html',
    styleUrls: ['./limitregister.component.css']
})
export class LimitregisterComponent implements OnInit {

    mid: string;
    limitlist: LimitRegisterViewModel;
    limitRegister: LimitRegister;

    constructor(private _route: ActivatedRoute,
                private _router: Router,
                private _walletService: WalletService,
                private _merchantService: MerchantService) {

        this.limitRegister = {
            MerchantId: this.mid,
            RegisterAmount: 0
        };

        this.limitlist = {
            MerchantName: "",
            MerchantNo: "",
            MerchantId: "",
            SuspenseReceipts: 0,
            TotalSalesAmount: 0,
            WithdrawalTotalAmount: 0,
            DepositTotalAmount: 0,
            PreviousBalanceAmount: 0
        };

        this._route.params.subscribe(params => {
            this.mid = params["mid"];
            console.log(this.mid);
            this.searchLimit();
        });


    }

    ngOnInit(): void {
    }

    searchLimit() {
        this.limitRegister.RegisterAmount  = 0;
        this._walletService.selectAvailableLimit(this.mid)
            .subscribe(data =>  this.limitlist = data);
    }

    getAvailableLimit(): number {
        return this.limitlist.SuspenseReceipts + this.limitlist.PreviousBalanceAmount
            + this.limitlist.DepositTotalAmount - this.limitlist.WithdrawalTotalAmount
            + this.limitRegister.RegisterAmount;
    }

    save() {
        if (!this.limitRegister.RegisterAmount) {
            alert("금회 입금액이 입력되어야 합니다");
            return;
        }

        if(this.limitRegister.RegisterAmount < 0) {
            alert("금회 입금액이 0보다 커야 됩니다");
            return;
        }

        if (!confirm("변경된 정보를 저장하시겠습니다"))
            return;

        console.log(this.limitRegister);

        this._walletService.registerLimit(this.limitRegister)
            .subscribe(x => {
                alert("변경되었습니다");
                this.searchLimit();
            })
    }

    goList() {
        this._router.navigate(['/limitList']);
    }
}

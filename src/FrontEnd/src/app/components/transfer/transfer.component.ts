import { Component, OnInit } from '@angular/core';
import {Withdrawal} from "../../models/Withdrawal";
import {AccountInquiry} from "../../models/AccountInquiry";
import {ActivatedRoute, Router} from "@angular/router";
import {UserService} from "../../service/user.service";
import {WalletService} from "../../service/wallet.service";
import {LimitRegisterViewModel} from "../../models/LimitRegisterViewModel";

@Component({
    selector: 'app-transfer',
    templateUrl: './transfer.component.html',
    styleUrls: ['./transfer.component.css']
})
export class TransferComponent implements OnInit {
    mid: string;
    withdrawal : Withdrawal;
    account: AccountInquiry;
    limit : LimitRegisterViewModel;
    accountResponseName: string;
    selectedBankName: string;
    accountInquiry: boolean = false;

    constructor(private _route: ActivatedRoute,
                private _router: Router,
                private _userService: UserService,
                private _walletService: WalletService) {
        this.withdrawal = {
            BankCode: "",
            AccountNo: "",
            AccountName: "",
            TransferAmount: 0,
            Name: "",
            OutName: "",
            MID: ""
        }

        this.limit = {
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
            this.withdrawal.MID = this.mid;
            this.searchLimit();
        });
    }

    searchLimit() {
        this._walletService.selectAvailableLimit(this.mid)
            .subscribe(data => this.limit = data);
    }

    ngOnInit(): void {
    }

    getAvailableLimit(): number {
        return this.limit.SuspenseReceipts + this.limit.PreviousBalanceAmount
            + this.limit.DepositTotalAmount - this.limit.WithdrawalTotalAmount;
    }

    goList() {
        this._router.navigate(["/limitList"]);
    }

    registerAccountInquiry() {
        if(!this.withdrawal.BankCode) {
            alert("입금하실 은행을 입력하여야 합니다");
            return;
        }

        if(!this.withdrawal.AccountNo) {
            alert("입금하실 계좌번호를 입력하여야 합니다");
            return;
        }

        if(this.withdrawal.TransferAmount <= 0) {
            alert("입금하실 금액은 0원보다 커야 됩니다");
            return;
        }


        if(!this.withdrawal.Name) {
            alert("입금하실메모를 입력하여야 합니다");
            return;
        }

        const inqury = {
            BankCode: this.withdrawal.BankCode,
            SearchAccountNo: this.withdrawal.AccountNo,
            AccountNo: "",
            TransferAmount: 0
        }

        this._userService.selectAccountInquiry(inqury)
            .subscribe(data => console.log(data));

        console.log(this.withdrawal);
    }

    registerTransfer() {
        if (!confirm("송금을 위한 개별등록을 하시겠습니까?"))
            return;

        this._userService.withdrawal(this.withdrawal)
            .subscribe(data => console.log(data));
    }

    onCancel() {
        alert("임수민 만세33");
    }
}

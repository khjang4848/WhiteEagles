import { Component, OnInit } from '@angular/core';
import {LimitList} from "../../../models/LimitList";
import {WalletService} from "../../../service/wallet.service";
import {MatTableDataSource} from "@angular/material/table";
import {Router} from "@angular/router";

@Component({
    selector: 'app-limitlist',
    templateUrl: './limitlist.component.html',
    styleUrls: ['./limitlist.component.css']
})
export class LimitlistComponent implements OnInit {
    limitlists: LimitList[] = [];
    mid: string;
    dataSource = new MatTableDataSource<LimitList>(this.limitlists);

    constructor(private _router: Router, private _walletService: WalletService) {
        this.mid = "";
    }

    ngOnInit(): void {
    }

    searchData() {
        this._walletService.selectAvailableLimitAll(this.mid)
            .subscribe(data => {
                this.limitlists = data;
                console.log(this.limitlists);
            });
    }

    searchExcel() {
        alert("임수민 만세!!");
    }

    goModifyLimit(mid) {
        this._router.navigate(["/limitRegister", mid]);
    }

    goRegister(mid) {
        this._router.navigate(["/transfer", mid]);
    }


}

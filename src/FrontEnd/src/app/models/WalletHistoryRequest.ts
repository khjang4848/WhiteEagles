export interface WalletHistoryRequest {
    Mid: string;
    StartDate: string;
    EndDate: string;
    RemiName: string;
    InAccountNo: string;
    BankCode: string;
    TransferStatus: string;
    Page: number;
    PageCount: number;
}

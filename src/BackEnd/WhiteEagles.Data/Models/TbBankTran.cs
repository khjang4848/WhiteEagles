namespace WhiteEagles.Data.Models
{
    using System.ComponentModel;

    /// <summary>
    /// 출금 요청 EMONEY-DB 스키마 : YKLEE
    /// </summary>
    public class TbBankTran
    {
        /// <summary>
        /// 요청 일자 : yyyyMMdd
        /// </summary>
        public string TrDate { get; set; }
        /// <summary>
        /// 일련번호 : 요청일자별로 일련번호 채번하여 셋팅
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// 가입 은행 코드
        /// </summary>
        public string OrgBank { get; set; }
        /// <summary>
        /// 가입 기관 코드
        /// </summary>
        public string OrgCd { get; set; }
        /// <summary>
        /// 거래 일련 번호 : 모듈에서 자동 셋팅
        /// </summary>
        public decimal TrSeq { get; set; }
        /// <summary>
        /// 출금은행 코드
        /// </summary>
        public string OutBankCd { get; set; }
        /// <summary>
        /// 출금계좌 번호
        /// </summary>
        public string OutAcctNo { get; set; }
        /// <summary>
        /// 입금은행 코드
        /// </summary>
        public string InBankCd { get; set; }
        /// <summary>
        /// 입금계좌 번호
        /// </summary>
        public string InAcctNo { get; set; }

        [DefaultValue(0)]
        /// <summary>
        /// 거래금액
        /// </summary>
        public int TrAmt { get; set; }
        /// <summary>
        /// 수수료
        /// </summary>
        public decimal? Fee { get; set; }
        /// <summary>
        /// 거래후 잔액 부호 : 0 or other
        /// </summary>
        public string BalSign { get; set; }
        /// <summary>
        /// 거래후 잔액
        /// </summary>
        public decimal? BalAmt { get; set; }
        /// <summary>
        /// 출금계좌 적요(메모)
        /// </summary>
        public string OutName { get; set; }
        /// <summary>
        /// 입금계좌 적요(메모)
        /// </summary>
        public string RemiName { get; set; }
        /// <summary>
        /// CMS 코드
        /// </summary>
        public string CmsCd { get; set; }
        /// <summary>
        /// 처리여부 플래그 : 송신(S), 미전송(N), 응답완료(Y)
        /// </summary>
        public string ProcFlag { get; set; }
        /// <summary>
        /// 오류 코드
        /// </summary>
        public string ErrorCd { get; set; }
        /// <summary>
        /// 타행불능 여부 : 타행불능시 Y
        /// </summary>
        public string Unable { get; set; }
        /// <summary>
        /// 송신 시간
        /// </summary>
        public string SendDatetime { get; set; }
        /// <summary>
        /// 수신 시간
        /// </summary>
        public string RecvDatetime { get; set; }
        /// <summary>
        /// 입력일자(등록일시) : yyyyMMddhhmmss
        /// </summary>
        public string EntryDate { get; set; }
        /// <summary>
        /// 입력자 : BR_ (Prefix BR_ 반드시 붙여주어야 한다.)
        /// </summary>
        public string EntryIdno { get; set; }
        /// <summary>
        /// ERP 적용 여부 : 적용(Y)
        /// </summary>
        public string ErpProcYn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? UseIdx { get; set; }
    }
}

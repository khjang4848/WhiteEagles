export const getFormatDate = (data: Date) => {
    const year = data.getFullYear();
    let month: number | string = (1 + data.getMonth());
    month = month > 10 ? month : "0" + month;
    let day: number | string = data.getDate();
    day = day >= 10 ? day : "0" + day;

    return  year + "" + month + "" + day;
};

export const getFormatDateDash = (data: Date) => {
    const year = data.getFullYear();
    let month: number | string = (1 + data.getMonth());
    month = month > 10 ? month : "0" + month;
    let day: number | string = data.getDate();
    day = day >= 10 ? day : "0" + day;

    return  year + "-" + month + "-" + day;
};

export const getFormatYearAndMonth = (data: Date) => {
    const year = data.getFullYear();
    let month: number | string = (1 + data.getMonth());
    month = month > 10 ? month : "0" + month;

    return  year + "" + month;
};

export const getConvertDate = (data: string) => {
    const year = data.substring(0, 4);
    const month = data.substring(5, 7);
    const day = data.substring(8, 10);

    return new Date(Number(year), Number(month)-1, Number(day));
};

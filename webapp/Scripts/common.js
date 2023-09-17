function ConvertDateFormat(date) {
    var html;
   
    if (date == null) {
        html = "";
    }
    else {
        html = kendo.toString(date, 'dd/MM/yyyy');
    }

    return html;
}
function ConvertDateTimeFormat(date2) {
    
    var html2;

    if (date2 == null) {
        html2= "";
    }
    else {
        html2 = kendo.toString(date2, 'dd/MM/yyyy HH:mm');
    }

    return html2;
}
function getYears()
{
    var date = new Date,
            years = [],
            year = date.getFullYear()-5;
    for (var i = year; i < year + 10; i++) {
        years.push(i);
    }
    return years;
};

function getMonths() {
    var items = [
        { text: "มกราคม", value: "01" },
        { text: "กุมภาพันธ์", value: "02" },
        { text: "มีนาคม", value: "03" },
        { text: "เมษายน", value: "04" },
        { text: "พฤษภาคม", value: "05" },
        { text: "มิถุนายน", value: "06" },
        { text: "กรกฎาคม", value: "07" },
        { text: "สิงหาคม", value: "08" },
        { text: "กันยายน", value: "09" },
        { text: "ตุลาคม", value: "10" },
        { text: "พฤศจิกายน", value: "11" },
        { text: "ธันวาคม", value: "12" }
    ];
    //var monthNames = ["มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
    //    "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"];
    return items;
};
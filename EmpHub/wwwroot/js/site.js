// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showPreloader() {
    $('#status').fadeIn();
    $('#preloader').fadeIn();
}

function hidePreloader() {
    //$('#img-pre').remove();
    $('#status').fadeOut();
    $('#preloader').fadeOut();
}

function ConvertToDate(dt) {
    var dt = dt.split(' ')[0];
    var parts = dt.split('/');
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

/* ตัวเลือก "อื่น ๆ" ที่ผู้ใช้พิมพ์ชื่อเอง แล้วเก็บชื่อลงคอลัมน์เดิมตรง ๆ
   ทำให้ *Desc ที่ proc join มาจะว่าง จุดแสดงผลจึงต้อง fallback ไปอ่านคอลัมน์ค่า

   sentinel มี 3 ตัวเพราะ select ในระบบเก็บค่าไม่เหมือนกัน
   - เก็บ code    (เช่น สัญชาติ, คำนำหน้าใน Onboarding) ใช้ MASTER_OTHER
   - เก็บ desc_th (เช่น คำนำหน้าในฟอร์ม Service)        ใช้ MASTER_OTHER_TH
   - เก็บ desc_en                                        ใช้ MASTER_OTHER_EN */
var MASTER_OTHER = 'OTHER';
var MASTER_OTHER_TH = 'อื่น ๆ (ระบุ)';
var MASTER_OTHER_EN = 'Other (specify)';

function MasterOptionListWithOther(lMaster) {
    var lData = (lMaster || []).slice();

    if (!lData.find(function (x) { return x.code == MASTER_OTHER; })) {
        lData.push({ code: MASTER_OTHER, desc_th: MASTER_OTHER_TH, desc_en: MASTER_OTHER_EN });
    }

    return lData;
}

// ค่าที่บันทึกไว้ไม่ตรงตัวเลือกใดในรายการ = เป็นชื่อที่ผู้ใช้พิมพ์เอง
// field คือคอลัมน์ที่ select นั้นใช้เป็นค่า ('code' | 'desc_th' | 'desc_en')
function MasterIsOther(lOption, value, field) {
    if (!value) {
        return false;
    }

    field = field || 'code';

    return !(lOption || []).some(function (x) { return x[field] == value; });
}

/* ลำดับชั้นหน่วยงาน ด้าน > สาย > ฝ่าย > ส่วน
   ระดับที่ไม่มีข้อมูลจะแสดง ... แทน เพื่อให้เห็นครบทุกกล่องว่าขาดระดับไหน */
var ORG_CHAIN_EMPTY = '...';

function OrgChainItems(iEmployee) {
    iEmployee = iEmployee || {};

    return [
        { label: 'ด้าน', name: iEmployee.functionDesc || ORG_CHAIN_EMPTY }
        , { label: 'สาย', name: iEmployee.divisionDesc || ORG_CHAIN_EMPTY }
        , { label: 'ฝ่าย', name: iEmployee.departmentDesc || ORG_CHAIN_EMPTY }
        , { label: 'ส่วน', name: iEmployee.sectionDesc || ORG_CHAIN_EMPTY }
    ];
}

/* สถานะความพิการที่ถือว่า "เป็นผู้พิการ" จะถูกทำเครื่องหมายไว้ที่ condition_1 = 'Y'
   ตั้งค่าได้เองที่หน้า Setting > Disability จึงเพิ่ม/แก้สถานะได้โดยไม่ต้องแก้โค้ด */
var DISABILITY_FLAG_YES = 'Y';

function IsDisabilityCardRequired(lDisability, value) {
    if (!value) {
        return false;
    }

    var find = (lDisability || []).find(function (x) { return x.code == value; });

    return !!find && (find.condition_1 || '').toUpperCase() == DISABILITY_FLAG_YES;
}

function NewGuid() {
    if (window.crypto && window.crypto.randomUUID) {
        return window.crypto.randomUUID().toUpperCase();
    }

    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0;
        var v = c == 'x' ? r : ((r & 0x3) | 0x8);
        return v.toString(16);
    }).toUpperCase();
}

function ConvertToDate103(dt) {
    var dd = dt.getDate();
    var mm = dt.getMonth() + 1;
    var yyyy = dt.getFullYear();

    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }

    return dd + '/' + mm + '/' + yyyy;
}

function JS_ValidateMobile(text) {
    if (!text) return false;

    var regex = /^([0-9]{3}-[0-9]{3}-[0-9]{4}|[0-9]{10})$/;

    return regex.test(text);
}

function JS_ValidateDate(text) {
    if (text == null || text === "") {
        return false;
    }

    text = text.trim();

    // ต้องเป็น dd/mm/yyyy เท่านั้น
    var regex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    var match = regex.exec(text);

    console.log(match);

    if (!match) {
        return false;
    }

    var day = parseInt(match[1], 10);
    var month = parseInt(match[2], 10);
    var year = parseInt(match[3], 10);

    // ตรวจช่วงเบื้องต้น
    if (year < 1900 || year > 9999) {
        return false;
    }

    if (month < 1 || month > 12) {
        return false;
    }

    if (day < 1 || day > 31) {
        return false;
    }

    // ตรวจวันที่จริง เช่น 31/02 ต้อง false
    var date = new Date(year, month - 1, day);

    return date.getFullYear() === year &&
        date.getMonth() === month - 1 &&
        date.getDate() === day;
}

function JS_IsValidDateDDMMYYYY(value) {
    if (!value) return false;

    if (moment.isMoment(value)) {
        value = value.format("DD/MM/YYYY");
    }

    if (!/^\d{2}\/\d{2}\/\d{4}$/.test(value)) {
        return false;
    }

    return moment(value, "DD/MM/YYYY", true).isValid();
}

function JS_ValidateTime(text) {
    var regex = /^([01][0-9]|2[0-3]):[0-5][0-9]$/;

    if (text == "" || text == null || regex.test(text) == false) {
        return false;
    } else {
        return true;
    }
}

function JS_ValidateEmail(text) {
    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (text == "" || text == null || regex.test(text) == false) {
        return false;
    } else {
        return true;
    }
}

function JS_Reset(inputbox) {
    inputbox.removeClass("input-error");
    inputbox.removeClass("is-invalid");
    inputbox.removeClass("is-valid");
}

function JS_Require(inputbox) {
    inputbox.addClass("input-error");
    inputbox.addClass("is-invalid");
}

function AjaxError(jqXHR, error, errorThrown) {
    hidePreloader();
    Swal.fire({
        title: "ระบบเกิดข้อผิดพลาด",
        text: jqXHR.responseText,
        icon: "error",
        confirmButtonColor: '#1e3b92',
    });
}

function alertSuccess(title, msg, fn) {
    Swal.fire({
        title: title,
        text: msg,
        icon: "success",
        allowOutsideClick: false,
        confirmButtonText: "ตกลง",
        confirmButtonColor: '#1e3b92',
    }).then((result) => {
        if (result.value) {
            if (fn) {
                fn();
            }
        }
    });
}

function alertConfirm(title, msg, fn) {
    Swal.fire({
        title: title,
        text: msg,
        icon: "warning",
        confirmButtonText: "ตกลง",
        confirmButtonColor: '#1e3b92',
        cancelButtonText: "ปิด",
        showCancelButton: true
    }).then((result) => {
        if (result.value) {
            if (fn) {
                fn();
            }
        }
    });
}

function alertWarning(title, msg, fn) {
    Swal.fire({
        title: title,
        text: msg,
        icon: "warning",
        confirmButtonText: "ตกลง",
        confirmButtonColor: '#1e3b92',
    }).then((result) => {
        if (fn) {
            fn();
        }
    });
}

function dlgInfo(title, msg) {
    var dialog = $("#dialog-info");
    var dialog_title = $(".dialog-info-title");
    var dialog_body = $(".dialog-info-body");

    dialog.modal({ backdrop: 'static', keyboard: false })
    dialog_title.html("");
    dialog_body.html("");
    dialog_title.html(title);
    dialog_body.html(msg);
    dialog.modal('show');
}

function dlgWarning(title, msg) {
    var dialog = $("#dialog-warning");
    var dialog_title = $(".dialog-warning-title");
    var dialog_body = $(".dialog-warning-body");

    dialog.modal({ backdrop: 'static', keyboard: false })
    dialog_title.html("");
    dialog_body.html("");
    dialog_title.html(title);
    dialog_body.html(msg);
    dialog.modal('show');
}

function setCookie(cName, cValue, expDays) {
    let date = new Date();
    date.setTime(date.getTime() + (expDays * 24 * 60 * 60 * 1000));
    const expires = "expires=" + date.toUTCString();
    document.cookie = cName + "=" + cValue + "; " + expires + "; path=/";
}

function getCookie(name) {
    const regex = new RegExp(`(^| )${name}=([^;]+)`)
    const match = document.cookie.match(regex)
    if (match) {
        return match[2]
    }
}

function getFileInfo(str) {
    if (str == null) { return null; }

    var file = str.split(';'); // data:image/png;base64,'

    if (file.length != 2) { return null; }

    var data = file[0];
    var base64 = file[1];

    return {
        base64: base64.split(',')[1]
        , filetype: data.split(':')[1]
    };
}

function FileSizeDesc(totalSize) {
    var code = ["bytes", "KB", "MB", "GB"];
    var index = 0;

    while (totalSize > 1024) {
        totalSize = (totalSize / 1024.0);
        index++;
    }

    if (totalSize > 0) {
        return parseFloat(totalSize).toFixed(2) + ' ' + code[index];
    } else {
        return '0 ' + code[index];
    }
}

function ExportFile(JSData, filename) {
    if (JSData != null) {
        var tableData = [
            {
                "sheetName": "Sheet1",
                "data": JSData
            }
        ];

        var rightNow = new Date();
        var res = rightNow.toISOString().slice(0, 10).replace(/-/g, "");

        var options = {
            fileName: filename
        };
        Jhxlsx.export(tableData, options);
    }
}

function auto_grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight + 10) + "px";
}

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}

Date.prototype.addMonths = function (months) {
    var n = this.getDate();
    this.setDate(1);
    this.setMonth(this.getMonth() + months);
    this.setDate(Math.min(n, this.getDaysInMonth()));
    return this;
};

Date.prototype.getDaysInMonth = function () {
    return Date.getDaysInMonth(this.getFullYear(), this.getMonth());
};

Date.getDaysInMonth = function (year, month) {
    return [31, (Date.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
};
$(document).ready(function () {

    $('.allow_numeric').keypress(function (event) {
        return isNumber(event, this);
        //if (isNumber(event, this)) {
        //    this.removeClass('form-control-error');
        //    return true;
        //}

        //if (!this.hasClass('form-control-error')) { this.addClass('form-control-error') }

        //return false;
    });

    $('.blockspecialcharacter').keypress(function (event) {
        return blockspecialcharacter(event)
    });

    $(".inputmask_Time").inputmask("hh:mm");
    $(".inputmask_Date").inputmask("dd/mm/yyyy");
    //$(".inputmask_Time").inputmask({ "alias": "Regex", "regex": "^([0-9]|0[0-9]|1[0-9]|2[0-4]):[0-5][0-9]$" });

    $('.select2').select2();
    //$('select').on('select2:open', function (e) {
    //    $('body').css('overflow', 'hidden');
    //});
});

function dispatch(fn, args) {
    fn = (typeof fn == "function") ? fn : window[fn];  // Allow fn to be a function object or the name of a global function
    return fn.apply(this, args || []);  // args is optional, use an empty array by default
}

function swalError(title, text) {
    Swal.fire({
        icon: 'error',
        title: title,
        text: text
    })
}

function swalSuccess(title, text) {
    Swal.fire({
        icon: 'success',
        title: title,
        text: text
    })
}

function swalConfirm(title, fn, args) {

    if (typeof title == 'undefined' || title == null || title == '') { title = 'Do you want to perform this action?'; }

    Swal.fire({
        title: title,
        showDenyButton: false,
        showCancelButton: true,
        confirmButtonText: 'Save'
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            dispatch(fn, args);
        } else {
            Swal.close();
        }
    })
}

function ValidateEmail_Border($this) {
    if (typeof $this != 'undefined' && $this != null) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test($this.value)) { $this.removeClass('form-control-error'); return true; }
    }

    if (!$this.hasClass('form-control-error')) { $this.addClass('form-control-error') }

    return false;
}

function ValidateEmail(strValue) {
    if (typeof strValue != 'undefined' && strValue != null) {
        if (/^(\+\d{1,3}[- ]?)?\d{10}$/.test(strValue)) { return true; }
    }
    swalError('Opps...!', "You have entered an invalid mobile number!")
    return false;
}

function ValidateMobileNo_Border($this) {
    if (typeof $this != 'undefined' && $this != null) {
        if (/^(\+\d{1,3}[- ]?)?\d{10}$/.test(strValue)) { $this.removeClass('form-control-error'); return true; }
    }

    if (!$this.hasClass('form-control-error')) { $this.addClass('form-control-error') }

    return false;
}

function ValidateMobileNo(strValue) {
    if (typeof strValue != 'undefined' && strValue != null) {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(strValue)) { return true; }
    }
    swalError('Opps...!', "You have entered an invalid email address!")
    return false;
}

function isNumber(evt, element) {

    if ($(element).val().indexOf('.') == 0) {
        $(element).val('0' + $(element).val());
        $(element).trigger('change');
        return true;
    }

    var charCode = (evt.which) ? evt.which : event.keyCode

    if (
        (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // - CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // . CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function blockspecialcharacter(e) {
    var key = document.all ? key = e.keyCode : key = e.which;
    return ((key > 64 && key < 91) || (key > 96 && key < 123) || key == 8 || key == 32 || (key >= 48 && key <= 57));
}

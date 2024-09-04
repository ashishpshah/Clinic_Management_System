var filesToUpload = null;

var dataTable_main = null;
var dataTable_Checkboxes = null;

$(document).ready(function () {

    var currentUrl = window.location.href.toString().replace(window.location.origin, '');

    if (typeof currentUrl != 'undefined' && currentUrl != null) {

        if (currentUrl == '/Home/Index')
            currentUrl = '/';

        var aTags = $('.nav-sidebar a[href="' + currentUrl + '"]');
        if (typeof aTags != 'undefined' && aTags != null && aTags.length > 0) {
            $(aTags[0]).addClass('active');
            try { $(aTags[0]).parents('ul.nav-treeview').parents('li.nav-item').addClass('menu-open'); } catch { }
            localStorage.clear();
            localStorage.setItem('active-tab', 'a[href="' + currentUrl + '"]');
        }
        else {
            aTags = $(localStorage.getItem('active-tab'));
            $(aTags[0]).addClass('active');
            try { $(aTags[0]).parents('ul.nav-treeview').parents('li.nav-item').addClass('menu-open'); } catch { }
        }
    }


    try {
        $('.select2').select2();

        $('select').on('select2:open', function () {
            var container = $('.select2-container').last();
        });

    } catch { }

    try {
        //Datemask dd/mm/yyyy
        $('.datemask_ddmmyyyy').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' })
    } catch { }

    try {

        //Datemask dd/mm/yyyy

        var locale_date_format = 'DD/MM/YYYY';

        $('.datepicker').each(function (i, e) {

            $(e).daterangepicker({
                singleDatePicker: true,
                autoclose: true,
                autoApply: false,
                //timePicker: true,
                //ranges: {
                //    'Today': [moment(), moment()],
                //    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                //    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                //    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                //    'This Month': [moment().startOf('month'), moment().endOf('month')],
                //    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                //},
                showDropdowns: true,
                //maxDate: moment().format('DD/MM/YYYY'),
                locale: { cancelLabel: 'Clear', format: locale_date_format/*, format: 'DD/MM/YYYY hh:mm A'*/ }
            }, function (start, end, label) { try { fnChangeDatePicker(this, start, end, label); } catch { } });


            var defaultDate = '';
            var minDate = '';
            var maxDate = '';

            if (e.hasAttribute('data-max') && typeof e.getAttribute('data-max') != 'undefined' && e.getAttribute('data-max') != null && e.getAttribute('data-max').length > 0) {
                maxDate = e.getAttribute('data-max');
            }

            if (e.hasAttribute('data-min') && typeof e.getAttribute('data-min') != 'undefined' && e.getAttribute('data-min') != null && e.getAttribute('data-min').length > 0) {
                minDate = e.getAttribute('data-min');
            }

            if (e.hasAttribute('data-value') && typeof e.getAttribute('data-value') != 'undefined' && e.getAttribute('data-value') != null && e.getAttribute('data-value').length > 0) {
                defaultDate = e.getAttribute('data-value');
            }

            $(e).data('daterangepicker').maxDate = moment(maxDate, locale_date_format)
            $(e).data('daterangepicker').minDate = moment(minDate, locale_date_format)

            $(e).val(defaultDate);

            $(e).trigger('change');
        })


        $('.datepicker').on('cancel.daterangepicker', function (ev, picker) {
            debugger;
            $(this).val('');
            $(this).trigger('change');
        });


    } catch { }

    try {
        $('[data-toggle="tooltip"]').tooltip();

        $('.contact_number').keyup(function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        });
        $('.numbers').keyup(function () {
            this.value = this.value.replace(/[^0-9\.]/g, '');
        });
        $('.decimal-number').keypress(function (event) {
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });
        $('.decimal-number3').keypress(function (event) {
            var $this = $(this);
            if ((event.which != 46 || $this.val().indexOf('.') != -1) && ((event.which < 48 || event.which > 57) && (event.which != 0 && event.which != 8))) {
                event.preventDefault();
            }
            var text = $(this).val();
            if (text.length === 18) {
                $(this).val(text + ".")
            }
            if (text.length == 6) {
                if (event.which != 46) {
                    if ($this.val().indexOf('.') == -1) {
                        event.preventDefault();
                    }
                }
            }
            if ((event.which == 46) && (text.indexOf('.') == -1)) {
                setTimeout(function () {
                    if ($this.val().substring($this.val().indexOf('.')).length > 3) {
                        $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
                    }
                }, 1);
            }
            if ((text.indexOf('.') == 18 && text.substring(text.indexOf('.')).length > 3)) {
                event.preventDefault();
            }
            if (((text.indexOf('.') != -1) && (text.substring(text.indexOf('.')).length > 3) && (event.which != 0 && event.which != 8) && ($(this)[0].selectionStart >= text.length - 3))) {
                event.preventDefault();
            }
        });
        $('.decimal-number2').keypress(function (event) {
            var $this = $(this);
            if ((event.which != 46 || $this.val().indexOf('.') != -1) && ((event.which < 48 || event.which > 57) && (event.which != 0 && event.which != 8))) {
                event.preventDefault();
            }
            var text = $(this).val();
            if (text.length === 18) {
                $(this).val(text + ".")
            }
            if (text.length == 8) {
                if (event.which != 46) {
                    if ($this.val().indexOf('.') == -1) {
                        event.preventDefault();
                    }
                }
            }
            if ((event.which == 46) && (text.indexOf('.') == -1)) {
                setTimeout(function () {
                    if ($this.val().substring($this.val().indexOf('.')).length > 2) {
                        $this.val($this.val().substring(0, $this.val().indexOf('.') + 2));
                    }
                }, 1);
            }
            if ((text.indexOf('.') == 18 && text.substring(text.indexOf('.')).length > 2)) {
                event.preventDefault();
            }
            if (((text.indexOf('.') != -1) && (text.substring(text.indexOf('.')).length > 2) && (event.which != 0 && event.which != 8) && ($(this)[0].selectionStart >= text.length - 2))) {
                event.preventDefault();
            }
        });

        $('.allow_numeric').keypress(function (event) {
            return isNumber(event, this)
        });

        //$('button').mouseup(function () { this.blur(); });
        //$('input[type=button]').mouseup(function () { this.blur(); });
        //$('input[type=submit]').mouseup(function () { this.blur(); });

        //$('[data-toggle="tooltip"]').tooltip({
        //    trigger: 'hover'
        //})

        //$('div.modal').on('show.bs.modal', function () {
        //    $('button').tooltip('toggleEnabled');
        //})


    } catch { }

    $('body').on('keypress', '.isNumberKey', function (evt) {
        var theEvent = evt || window.event;

        // Handle paste
        if (theEvent.type === 'paste') {
            key = event.clipboardData.getData('text/plain');
        } else {
            // Handle key press
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
        }
        var regex = /^[0-9]*$/;
        if (!regex.test(key)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault) theEvent.preventDefault();
        }
    });

    $('body').on('keypress', '.isNumberKey_Decimal', function (evt, obj) {

        var theEvent = evt || window.event;
        var charCode = (evt.which) ? evt.which : event.keyCode;

        // Handle paste
        if (theEvent.type === 'paste') {
            key = event.clipboardData.getData('text/plain');
        } else {
            // Handle key press
            var key = theEvent.keyCode || theEvent.which;
            key = String.fromCharCode(key);
        }

        var dotcontains = theEvent.target.value.indexOf(".") != -1;
        if (dotcontains)
            if (charCode == 46) {
                theEvent.returnValue = false;
                if (theEvent.preventDefault) theEvent.preventDefault();
                return false;
            };

        var regex = /^[0-9]*$/;
        if (!regex.test(key) && !(charCode == 46)) {
            theEvent.returnValue = false;
            if (theEvent.preventDefault) theEvent.preventDefault();
            return false;
        }

    });

    $('body').on('click', '.btnSubmit', function (e) {
        e.preventDefault();
        fnSubmitForm($(this).parents('form').attr('id'));
    });

    $('.modal').on('shown.bs.modal', function (e) {

        $('body').append('<div class="modal-backdrop fade show" id="div-modal-backdrop"></div>')

    });

    $('.modal').on('hide.bs.modal', function (e) {

        $('body div#div-modal-backdrop').remove();

        $('div.loader-overlay').remove();
        if (!($(document.activeElement)[0].type == 'button' && $($(document.activeElement)[0]).hasClass('close')))
            e.preventDefault();
    });

    try { fnLoadCommonTable('#table_Common'); } catch { }
    try { fnLoadCommonTable('.table_Common'); } catch { }

    try { fnLoadCommonTable_ScrollX('#table_Common_ScrollX'); } catch { }
    try { fnLoadCommonTable_ScrollX('.table_Common_ScrollX'); } catch { }

    try { fnLoadCommonTable_SrNo('#table_Common_SrNo'); } catch { }
    try { fnLoadCommonTable_SrNo('.table_Common_SrNo'); } catch { }

    try { fnLoadCommonTable_Buttons('#table_Common_buttons'); } catch { }
    try { fnLoadCommonTable_Buttons('.table_Common_buttons'); } catch { }

    try { fnFileUpload(".input-file"); } catch { }

});

function ajaxPost($url, $data) {

    ShowLoader(true);

    $.ajax({
        type: 'POST',
        url: $url,
        data: $data,
        cache: false,
        contentType: false,
        processData: false,
        dataType: "json",
        success: function (response) {

            try {
                ShowLoader(false);
                if (response.statusCode === 1) {
                    if (typeof response.isConfirm != 'undefined' && response.isConfirm != '' && response.isConfirm != null && response.isConfirm == true)
                        if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                            CommonConfirmed_Success(response.message, response.redirectURL, null);
                        else
                            CommonConfirmed_Success(response.message, fnSubmitForm_Success, [response, 'AJAX']);
                    else
                        if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                            window.location = response.redirectURL;
                        else
                            fnSubmitForm_Success(response, 'AJAX');
                }
                else CommonAlert_Error(response.message, null);
            } catch { window.location.reload(); }
        },
        //xhr: function () {
        //    var fileXhr = $.ajaxSettings.xhr();
        //    if (fileXhr.upload) {
        //        $("progress").show();
        //        fileXhr.upload.addEventListener("progress", function (e) {
        //            if (e.lengthComputable) {
        //                $("#fileProgress").attr({
        //                    value: e.loaded,
        //                    max: e.total
        //                });
        //            }
        //        }, false);
        //    }
        //    return fileXhr;
        //},
        failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
        error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
    });
}

function ajaxGet($url, $isReturnToForm, $redirectUrl) {

    ShowLoader(true);

    $.ajax({
        type: 'GET',
        url: $url,
        data: null,
        cache: false,
        contentType: false,
        processData: false,
        dataType: "json",
        success: function (response) {

            try {
                ShowLoader(false);
                if (response.statusCode === 1) {
                    if (typeof response.isConfirm != 'undefined' && response.isConfirm != '' && response.isConfirm != null && response.isConfirm == true)
                        if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                            CommonConfirmed_Success(response.message, response.redirectURL, null);
                        else
                            if (typeof $isReturnToForm != 'undefined' && $isReturnToForm != '' && $isReturnToForm != null && $isReturnToForm == true)
                                CommonConfirmed_Success(response.message, fnSubmitForm_Success, [response, 'AJAX']);
                            else
                                if (typeof $redirectUrl != 'undefined' && $redirectUrl != '' && $redirectUrl != null && $redirectUrl.length > 0)
                                    CommonConfirmed_Success(response.message, $redirectUrl, null);
                                else
                                    CommonConfirmed_Success(response.message, response.redirectURL, null);
                    else
                        if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                            window.location = response.redirectURL;
                        else
                            if (typeof $isReturnToForm != 'undefined' && $isReturnToForm != '' && $isReturnToForm != null && $isReturnToForm == true)
                                fnSubmitForm_Success(response, 'AJAX');
                            else
                                if (typeof $redirectUrl != 'undefined' && $redirectUrl != '' && $redirectUrl != null && $redirectUrl.length > 0)
                                    window.location = $redirectUrl;
                                else
                                    window.location = response.redirectURL;
                }
                else {
                    ShowLoader(false); CommonAlert_Error(response.message, null);
                }
            } catch { ShowLoader(false); window.location.reload(); }
        },
        //xhr: function () {
        //    var fileXhr = $.ajaxSettings.xhr();
        //    if (fileXhr.upload) {
        //        $("progress").show();
        //        fileXhr.upload.addEventListener("progress", function (e) {
        //            if (e.lengthComputable) {
        //                $("#fileProgress").attr({
        //                    value: e.loaded,
        //                    max: e.total
        //                });
        //            }
        //        }, false);
        //    }
        //    return fileXhr;
        //},
        failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
        error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
    });
}

function fnFileUpload($selector) {

    $($selector).before(function () {
        if (!$(this).prev().hasClass("input-ghost")) {

            var element = $("<input type='file' class='input-ghost' style='visibility:hidden; height:0; display: none;'>");

            if (($(this).find("input")[0]).hasAttribute('id'))
                element.attr("id", ($(this).find("input")[0]).getAttribute('id').replace('temp_', ''));

            if (($(this).find("input")[0]).hasAttribute('name'))
                element.attr("name", ($(this).find("input")[0]).getAttribute('name').replace('temp_', ''));

            if (($(this).find("input")[0]).hasAttribute('onchange'))
                element.attr("onchange", ($(this).find("input")[0]).getAttribute('onchange'));

            if (($(this).find("input")[0]).hasAttribute('accept'))
                element.attr("accept", ($(this).find("input")[0]).getAttribute('accept'));

            if (($(this).find("input")[0]).hasAttribute('data-required'))
                element[0].setAttribute("data-required", null);

            if (($(this).find("input")[0]).hasAttribute('data-msg'))
                element.attr("data-msg", ($(this).find("input")[0]).getAttribute('data-msg'));

            element.change(function () { element.next(element).find("input").val(element.val().split("\\").pop()); });

            $(this).find("button.btn-choose").click(function () { element.click(); });
            $(this).find("button.btn-reset").click(function () { element.val(null); $(this).parents($selector).find("input").val(""); });
            $(this).find("input").css("cursor", "pointer");
            $(this).find("input").mousedown(function () { $(this).parents($selector).prev().click(); return false; });
            $(this).find("input").keypress(function (evt) {

                var theEvent = evt || window.event;

                // Handle key press
                var key = theEvent.keyCode || theEvent.which;

                // Handle paste
                if (theEvent.type === 'paste') {
                    theEvent.returnValue = false;
                } else {
                    key = theEvent.keyCode || theEvent.which;
                }

                if (key != 13) {
                    theEvent.returnValue = false;
                    if (theEvent.preventDefault) theEvent.preventDefault();
                }
                else {
                    $(this).parents($selector).prev().click(); return false;
                }
            });

            return element;
        }
    });
}

function GetMonthName(monthNumber) {
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    return months[monthNumber - 1];
}

function fnShow_Password($id, $id_toggle) {

    const togglePassword = document.querySelector('#' + $id_toggle);
    const password = document.querySelector('#' + $id);
    const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
    password.setAttribute('type', type);
    togglePassword.classList.toggle('fa-eye-slash');
}

function ShowLoader(isShow) {

    if (isShow == true) {
        $('.preloader_new').removeClass('d-none');
        var elem = document.createElement('div');
        elem.className = "loader-overlay";
        elem.style.cssText = 'position: fixed;top: 0; left: 0; z-index: 999999; width: 100vw; height: 100vh; background-color: rgb(0, 0, 0, .5);';
        document.body.appendChild(elem);
    }
    else {
        $('.preloader_new').addClass('d-none');
        $('div.loader-overlay').remove();
    }
}

function fnLoadParialView($id, url) {

    ShowLoader(true);

    try {
        if (typeof $id != 'undefined' && $id != null && $id.length > 0) {

            //$('#' + $id).load(url, function () {

            //});

            //window.scrollTo(0, 0);

            fnCloseParialView($id);

            $.ajax({
                type: "GET",
                url: url,
                data: null,
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {

                    $('#' + $id).html('');
                    $('#' + $id).append(response);

                    setTimeout(function () {

                        $('#' + $id + ' .select2').select2();

                        try { fnFileUpload(".input-file"); } catch { }
                        try { fnFileUpload(".input-file-partial"); } catch { }

                        try { $('#' + $id).closest('.' + $id + '_Display').removeClass('d-none'); } catch { }

                        try { $('#' + $id).closest('.' + $id + '_Hide').addClass('d-none'); } catch { }


                        try {

                            //Datemask dd/mm/yyyy

                            var locale_date_format = 'DD/MM/YYYY';

                            $('#' + $id + ' .datepicker').each(function (i, e) {

                                $(e).daterangepicker({
                                    singleDatePicker: true,
                                    autoclose: true,
                                    autoApply: true,
                                    //timePicker: true,
                                    //ranges: {
                                    //    'Today': [moment(), moment()],
                                    //    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                                    //    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                                    //    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                                    //    'This Month': [moment().startOf('month'), moment().endOf('month')],
                                    //    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                                    //},
                                    showDropdowns: true,
                                    //maxDate: moment().format('DD/MM/YYYY'),
                                    locale: { cancelLabel: 'Clear', format: locale_date_format/*, format: 'DD/MM/YYYY hh:mm A'*/ }
                                }, function (start, end, label) { try { fnChangeDatePicker(this, start, end, label); } catch { } });


                                var defaultDate = '';
                                var minDate = '';
                                var maxDate = '';

                                if (e.hasAttribute('data-max') && typeof e.getAttribute('data-max') != 'undefined' && e.getAttribute('data-max') != null && e.getAttribute('data-max').length > 0) {
                                    maxDate = e.getAttribute('data-max');
                                }

                                if (e.hasAttribute('data-min') && typeof e.getAttribute('data-min') != 'undefined' && e.getAttribute('data-min') != null && e.getAttribute('data-min').length > 0) {
                                    minDate = e.getAttribute('data-min');
                                }

                                if (e.hasAttribute('data-value') && typeof e.getAttribute('data-value') != 'undefined' && e.getAttribute('data-value') != null && e.getAttribute('data-value').length > 0) {
                                    defaultDate = e.getAttribute('data-value');
                                }

                                $(e).val(defaultDate);

                                $(e).data('daterangepicker').maxDate = moment(maxDate, locale_date_format)
                                $(e).data('daterangepicker').minDate = moment(minDate, locale_date_format)

                                $(e).trigger('change');
                            })

                        } catch { }


                        try { fnLoadCommonTable('#table_Common'); } catch { }
                        try { fnLoadCommonTable('.table_Common'); } catch { }

                        try { fnLoadCommonTable_SrNo('#table_Common_SrNo'); } catch { }
                        try { fnLoadCommonTable_SrNo('.table_Common_SrNo'); } catch { }

                        try { fnLoadCommonTable_Buttons('#table_Common_buttons'); } catch { }
                        try { fnLoadCommonTable_Buttons('.table_Common_buttons'); } catch { }

                        try { fnParialView_Loaded_Success($id, (response.indexOf("Alert") > -1)); } catch { }


                        $('html, body').scrollTop($('#' + $id).offset().top);

                        ShowLoader(false);

                    }, 1000);

                },
                failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
                error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
            });
        }
    }
    catch (err) {
        ShowLoader(false);
    }

}

function fnCloseParialView($id, $reload_Id) {

    $('#' + $id).html('');
    $('.' + $id + '_Hide').removeClass('d-none');
    $('.' + $id + '_Display').addClass('d-none');

    if (typeof $reload_Id != 'undefined' && $reload_Id != null && $reload_Id.length != 0) {

        if ($reload_Id.indexOf('SrNo') > -1)
            try { fnLoadCommonTable_SrNo('#table_Common_SrNo'); } catch { }
        else if ($reload_Id.indexOf('buttons') > -1)
            try { fnLoadCommonTable_Buttons('#table_Common_buttons'); } catch { }
        else
            try { fnLoadCommonTable('#table_Common'); } catch { }

    }
}

function fnClearFormData($selector) {

    $.each($($selector + ' select'), function (key, input) {
        $('#' + $(input).attr('id') + ' option:selected').val('');
        $('#' + $(input).attr('id') + ' option:selected').trigger('change');
    });

    $.each($($selector + ' input'), function (key, input) {
        $('#' + $(input).attr('id')).val('');
        $('#' + $(input).attr('id')).trigger('change');
    });

    try { fnClearFormData_Success($selector); } catch { }

}

function formValidate($id) {

    var IsValid = true;
    //$.each($('#' + $this.closest('form').id + ' input[data-required]'), function (key, input) {
    $.each($($id + ' input[data-required]'), function (key, input) {
        if ((typeof input.value == 'undefined' || input.value == null || input.value.length == 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp_fileUpload')) {
            Swal.fire({ icon: 'error', title: input.getAttribute('data-msg') });
            IsValid = false;
            input.focus();
            return IsValid;
        }
    });

    $.each($($id + ' select[data-required]'), function (key, input) {
        if ((typeof input.value == 'undefined' || input.value == null || input.value.length == 0 || input.value == "0") && !$(input)[0].hasAttribute('disabled')) {
            Swal.fire({ icon: 'error', title: input.getAttribute('data-msg') });
            IsValid = false;
            input.focus();
            return IsValid;
        }
    });

    //if (IsValid)
    //    fnSubmitForm($this.closest('form').id);
    //return true;
    return IsValid;
}

function fnSubmitForm($id) {
    ShowLoader(true);

    var $form = $('#' + $id);

    if (formValidate('#' + $id)) {

        let formData = new FormData();

        const array = $form.serializeArray();

        $.each($('#' + $id + ' select'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $('#' + $(input).attr('id') + ' option:selected').val()
                    }
                })
        });

        $.each($('#' + $id + ' input[type="checkbox"]'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $(input).is(':checked')
                    }
                })
        });

        $.each($('#' + $id + ' input[type="radio"]'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $('input[name="' + $(input).attr('name') + '"]:checked').val()
                    }
                })
        });

        $.each(array, function (key, input) {
            if (typeof input.name != 'undefined' && input.name != null && input.name.length > 0 && input.name != "__RequestVerificationToken")
                formData.append(input.name, input.value);
        });

        var inputFiles = $('#' + $id + ' input[type="file"]');

        if (typeof inputFiles != 'undefined' && inputFiles != null && inputFiles.length > 0)
            $.each(inputFiles, function (key, input) {
                if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp') && !$(input).hasClass('temp_fileUpload')) {
                    var file = document.getElementById('' + input.getAttribute('id')).files[0];
                    if (typeof input.getAttribute('name') == 'undefined' || input.getAttribute('name') == null || input.getAttribute('name').length == 0) {
                        formData.append("files", file);
                    } else {
                        formData.append(input.getAttribute('name'), file);
                    }
                }
            });

        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            dataType: "json",
            success: function (response) {

                try {
                    ShowLoader(false);
                    if (response.statusCode === 1) {
                        if (typeof response.isConfirm != 'undefined' && response.isConfirm != '' && response.isConfirm != null && response.isConfirm == true)
                            if (typeof response.data5 != 'undefined' && response.data5 != '' && response.data5 != null)
                                if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                                    CommonConfirmed_Success_Print(response.message, response.data5, response.redirectURL, null);
                                else
                                    CommonConfirmed_Success_Print(response.message, response.data5, fnSubmitForm_Success, [response]);
                            else
                                if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                                    CommonConfirmed_Success(response.message, response.redirectURL, null);
                                else
                                    CommonConfirmed_Success(response.message, fnSubmitForm_Success, [response, $id]);
                        else
                            if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                                window.location = response.redirectURL;
                            else
                                fnSubmitForm_Success(response, $id);
                    }
                    else {
                        CommonAlert_Error(response.message, null)
                    }
                } catch {
                    window.location.reload();
                }
            },
            //xhr: function () {
            //    var fileXhr = $.ajaxSettings.xhr();
            //    if (fileXhr.upload) {
            //        $("progress").show();
            //        fileXhr.upload.addEventListener("progress", function (e) {
            //            if (e.lengthComputable) {
            //                $("#fileProgress").attr({
            //                    value: e.loaded,
            //                    max: e.total
            //                });
            //            }
            //        }, false);
            //    }
            //    return fileXhr;
            //},
            failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
            error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
        });

    }
    else { ShowLoader(false); return false; }

}

function fnSubmitForm_WithoutAlert($id) {
    ShowLoader(true);

    var $form = $('#' + $id);

    if (formValidate('#' + $id)) {

        let formData = new FormData();

        const array = $form.serializeArray();

        $.each($('#' + $id + ' select'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $('#' + $(input).attr('id') + ' option:selected').val()
                    }
                })
        });

        $.each($('#' + $id + ' input[type="checkbox"]'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp_fileUpload'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $(input).is(':checked')
                    }
                })
        });

        $.each($('#' + $id + ' input[type="radio"]'), function (key, input) {
            if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp_fileUpload'))
                array.filter(function (obj) {
                    if (obj['name'] == $(input).attr('name')) {
                        obj['value'] = $('input[name="' + $(input).attr('name') + '"]:checked').val()
                    }
                })
        });

        $.each(array, function (key, input) {
            if (typeof input.name != 'undefined' && input.name != null && input.name.length > 0 && input.name != "__RequestVerificationToken")
                formData.append(input.name, input.value);
        });

        var inputFiles = $('#' + $id + ' input[type="file"]');

        if (typeof inputFiles != 'undefined' && inputFiles != null && inputFiles.length > 0)
            $.each(inputFiles, function (key, input) {
                if ((typeof input.value != 'undefined' && input.value != null && input.value.length > 0) && !input.hasAttribute('disabled') && !$(input).hasClass('temp_fileUpload')) {
                    var file = document.getElementById('' + input.getAttribute('id')).files[0];
                    if (typeof input.getAttribute('name') == 'undefined' || input.getAttribute('name') == null || input.getAttribute('name').length == 0) {
                        formData.append("files", file);
                    } else {
                        formData.append(input.getAttribute('name'), file);
                    }
                }
            });

        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            dataType: "json",
            success: function (response) {
                try {
                    ShowLoader(false);
                    if (response.statusCode === 1)
                        if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                            window.location = response.redirectURL;
                        else
                            fnSubmitForm_Success(response, $id);
                    else
                        CommonAlert_Error(response.message, null);
                } catch { window.location.reload(); }
            },
            failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
            error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
        });

    }
    else { ShowLoader(false); return false; }

}

function fnSubmitForm_Confirm($id) {

    Swal.fire({
        icon: 'warning',
        title: "You are not able to update any data after submit application. Are you sure?",
        showDenyButton: false,
        showCancelButton: true,
        confirmButtonText: 'OK'
    }).then((result) => {

        if (result != null && result.isConfirmed == true)
            fnSubmitForm($id);
        //else if (result.isDenied)
        //    window.location.reload();
    });
}

function fnDelete_Confirm(url) {

    Swal.fire({
        icon: "warning",
        title: "Are you sure to delete this data?",
        //type: "error",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes!',
        //closeOnConfirm: false
    }).then((result) => {

        try {

            if (result != null && result.isConfirmed == true) {
                ShowLoader(true);

                $.ajax({
                    type: "POST",
                    url: url,
                    data: null,
                    success: function (response) {
                        ShowLoader(false);

                        try {
                            if (response.statusCode === 1) {
                                if (typeof response.isConfirm != 'undefined' && response.isConfirm != '' && response.isConfirm != null && response.isConfirm == true)
                                    if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                                        CommonConfirmed_Success(response.message, response.redirectURL, null);
                                    else
                                        CommonConfirmed_Success(response.message, fnDelete_Success, [response]);
                                else
                                    if (typeof response.redirectURL != 'undefined' && response.redirectURL != '' && response.redirectURL != null)
                                        window.location = response.redirectURL;
                                    else fnDelete_Success(response);
                            }
                            else CommonAlert_Error(response.message, null);
                        } catch { window.location.reload(); }
                    },
                    //xhr: function () {
                    //    var fileXhr = $.ajaxSettings.xhr();
                    //    if (fileXhr.upload) {
                    //        $("progress").show();
                    //        fileXhr.upload.addEventListener("progress", function (e) {
                    //            if (e.lengthComputable) {
                    //                $("#fileProgress").attr({
                    //                    value: e.loaded,
                    //                    max: e.total
                    //                });
                    //            }
                    //        }, false);
                    //    }
                    //    return fileXhr;
                    //},
                    failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
                    error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
                });
            }

        } catch { }
    });
}

//function CommonAlert_Error(msg) {
//    

//    if (msg == null || msg == "")
//        msg = "Oops...! Something went wrong!";

//    Swal.fire({ icon: 'error', title: msg })
//}

function CommonAlert_Error(msg, redirectUrl) {

    if (msg == null || msg == "")
        msg = "Oops...! Something went wrong!";

    Swal.fire({
        icon: 'error',
        title: msg,
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: 'OK'
    }).then((result) => {

        if (typeof redirectUrl != 'undefined' && redirectUrl != null && redirectUrl != '')
            window.location = redirectUrl;

        ShowLoader(false);
    })
}

function CommonAlert_Success(msg) {

    if (msg == null || msg == "")
        msg = "Data Successfully saved.";

    Swal.fire({
        icon: 'success',
        title: msg,
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: 'OK',
    });
}

function CommonConfirmed_Success(msg, functionName, functionParams) { //params = [2, 3, 'xyz'];

    if (msg == null || msg == "")
        msg = "Data Successfully saved.";

    Swal.fire({
        icon: 'success',
        title: msg,
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: 'OK'
    }).then((result) => {

        if (result.isConfirmed && typeof functionName != 'undefined' && functionName != null && functionName != '')
            if (typeof functionParams != 'undefined' && functionParams != null)
                if (Array.isArray(functionParams) && functionParams.length > -1) {
                    this.callback = functionName;
                    this.callback.apply(this, functionParams);
                }
                else this.callback = functionName;
            else {
                ShowLoader(true);
                window.location = functionName;
            }

        /* Read more about isConfirmed, isDenied below
        if (result.isConfirmed) {
            Swal.fire('Saved!', '', 'success')
        } else if (result.isDenied) {
            Swal.fire('Changes are not saved', '', 'info')
        } */

    })
}

function CommonConfirmed_Success_Print(msg, $url_print, functionName, functionParams) { //params = [2, 3, 'xyz'];

    if (msg == null || msg == "")
        msg = "Data Successfully saved.";

    Swal.fire({
        icon: 'success',
        title: msg,
        showDenyButton: true,
        showCancelButton: false,
        confirmButtonText: 'OK',
        denyButtonText: `Print Slip`
    }).then((result) => {

        debugger;

        if (result.isConfirmed && typeof functionName != 'undefined' && functionName != null && functionName != '')
            if (typeof functionParams != 'undefined' && functionParams != null)
                if (Array.isArray(functionParams) && functionParams.length > -1) {
                    this.callback = functionName;
                    this.callback.apply(this, functionParams);
                }
                else this.callback = functionName;
            else {
                ShowLoader(true);
                window.location = functionName;
            }
        else if (result.isDenied == true && typeof $url_print != 'undefined' && $url_print != null && $url_print != ''
            && typeof functionName != 'undefined' && functionName != null && functionName != '') {

            window.open($url_print, '_blank');

            if (typeof functionParams != 'undefined' && functionParams != null)
                if (Array.isArray(functionParams) && functionParams.length > -1) {
                    this.callback = functionName;
                    this.callback.apply(this, functionParams);
                }
                else this.callback = functionName;
            else {
                ShowLoader(true);
                window.location = functionName;
            }
        }

        /* Read more about isConfirmed, isDenied below
        if (result.isConfirmed) {
            Swal.fire('Saved!', '', 'success')
        } else if (result.isDenied) {
            Swal.fire('Changes are not saved', '', 'info')
        } */

        ShowLoader(false);
    })
}

function CallBack(fn, data, Id) {
    return fn(data, Id);
}

function fnChange_Switch($this, $labelId) {

    var attr_true = $("#" + $labelId).attr('data-true');
    var attr_false = $("#" + $labelId).attr('data-false');

    if ($this.checked) {
        $("#" + $labelId).html(attr_true);  // checked
    }
    else {
        $("#" + $labelId).html(attr_false);  // unchecked
    }

    try { fnChange_Switch_Success($this, $labelId); } catch { }
}

function fnLoadCommonTable($selector) {

    if ($.fn.DataTable.isDataTable($selector)) {
        $($selector).DataTable().destroy();
    }

    $($selector).DataTable({
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: true,
        responsive: true,
        pageLength: 25,
        lengthMenu: [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ],
        columnDefs: [
            { "targets": 0, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false },
            { "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
        ],
        fixedColumns: true,
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
    });

    $($selector + " thead th.no_sorting").removeClass('sorting');
    $($selector + " thead th.no_sorting").removeClass('sorting_asc');
    $($selector + " thead th.no_sorting").removeClass('sorting_desc');
}

function fnLoadCommonTable_ScrollX($selector) {

    if ($.fn.DataTable.isDataTable($selector)) {
        $($selector).DataTable().destroy();
    }

    $($selector).DataTable({
        scrollX: true,
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: true,
        responsive: true,
        pageLength: 25,
        lengthMenu: [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ],
        columnDefs: [
            { "targets": 0, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false },
            { "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
        ],
        fixedColumns: true,
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
    });

    $($selector + " thead th.no_sorting").removeClass('sorting');
    $($selector + " thead th.no_sorting").removeClass('sorting_asc');
    $($selector + " thead th.no_sorting").removeClass('sorting_desc');
}

function fnLoadCommonTable_Checkboxes($selector) {

    if ($.fn.DataTable.isDataTable($selector)) {
        $($selector).DataTable().destroy();
    }

    dataTable_Checkboxes = $($selector).DataTable({
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: true,
        responsive: true,
        pageLength: 25,
        lengthMenu: [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ],
        columnDefs: [
            //{ "targets": 0, "className": "select-checkbox text-center", "width": "3%", "checkboxes": { 'selectRow': true }, "autoWidth": false, "searchable": false, "orderable": false },
            {
                'targets': 0,
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(data).html() + '">';
                }
            },
            { "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
        ],
        select: { 'style': 'multi' },
        fixedColumns: true,
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
    });

    $($selector + " thead th.no_sorting").removeClass('sorting');
    $($selector + " thead th.no_sorting").removeClass('sorting_asc');
    $($selector + " thead th.no_sorting").removeClass('sorting_desc');


}

function fnLoadCommonTable_SrNo($selector) {

    if ($.fn.DataTable.isDataTable($selector)) {
        $($selector).DataTable().destroy();
    }

    $($selector).DataTable({
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: true,
        responsive: true,
        pageLength: 25,
        lengthMenu: [
            [10, 25, 50, -1],
            [10, 25, 50, 'All']
        ],
        columnDefs: [
            { "targets": 0, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false },
            { "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
        ],
        fixedColumns: true,
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            var index = iDisplayIndexFull + 1;
            $("td:first", nRow).html(index);
            return nRow;
        }
    });

    $($selector + " thead th.no_sorting").removeClass('sorting');
    $($selector + " thead th.no_sorting").removeClass('sorting_asc');
    $($selector + " thead th.no_sorting").removeClass('sorting_desc');
}

function fnLoadCommonTable_Buttons($selector) {

    if ($.fn.DataTable.isDataTable($selector)) {
        $($selector).DataTable().destroy();
    }

    var $title = $($selector).attr('data-export-file-name');


    if (typeof $title == 'undefined' || $title == null || $title == '')
        $title = 'Data export';

    $($selector).DataTable({
        paging: true,
        lengthChange: true,
        searching: true,
        ordering: true,
        info: true,
        autoWidth: true,
        responsive: true,
        columnDefs: [
            { "targets": 0, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false },
            { "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
        ],
        fixedColumns: true,
        dom: "<'row'<'col-sm-6 mb-3'B>>" +
            "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        //buttons: [{ extend: "csv", title: $title, className: "mr-2" },
        //{ extend: "excel", title: $title, className: "mr-2" },
        //{ extend: "pdfHtml5", title: $title, className: "mr-2" }]
        buttons: [{
            extend: "csv",
            className: "btn-flat btn-info mx-2 px-3",
            titleAttr: 'Export in CSV',
            text: 'CSV',
            filename: $title,
            init: function (api, node, config) { $(node).removeClass('btn-default') }
        },
        {
            extend: "excel",
            className: "btn-flat btn-success mx-2 px-3",
            titleAttr: 'Export in Excel',
            text: 'Excel',
            filename: $title,
            init: function (api, node, config) { $(node).removeClass('btn-default') }
        },
        {
            extend: "pdfHtml5",
            className: "btn-flat btn-danger mx-2 px-3",
            titleAttr: 'Export in PDF',
            text: 'PDF',
            exportOptions: {
                modifier: {
                    page: 'all' // Export all pages
                }
            },
            filename: $title,
            init: function (api, node, config) { $(node).removeClass('btn-default') },
            customize: function (doc) {
                doc.defaultStyle.fontSize = 10;
                doc.pageMargins = [20, 20, 20, 20]; // Adjust margins
                doc.pageOrientation = 'landscape'; // Set orientation
                // Add more customization options as needed
            }
        }]
    });

    $($selector + " thead th.no_sorting").removeClass('sorting');
    $($selector + " thead th.no_sorting").removeClass('sorting_asc');
    $($selector + " thead th.no_sorting").removeClass('sorting_desc');
}

function fnShow_Modal($url, $title, $hasTable, $type, $noAlert = false) {
    $('div.loader-overlay').remove();
    ShowLoader(true);

    if ($('#modal-xl').hasClass('show')) $('#modal-xl').hide(); //$('#modal-xl').modal('hide');

    if (typeof $url != 'undefined' && $url != null && $url != '' && $url.trim().length > 0)
        $.ajax({
            type: "GET",
            url: $url,
            data: null,
            contentType: "application/json; charset=utf-8",
            dataType: $type,
            success: function (response) {

                if (typeof response == 'undefined' || response == null || response == 'null' || response.length <= 0) {

                    if ($noAlert == false)
                        CommonAlert_Error("No any record(s) found");
                    return false;
                }


                $('#modal-xl .modal-body').html('');
                $('#modal-xl .modal-body').append(response);

                $('#modal-xl .modal-title').html($title);


                setTimeout(function () {

                    ShowLoader(false);

                    if ($hasTable === true) {

                        try {

                            if ($.fn.DataTable.isDataTable('#modal-xl .modal-body .table_Common')) {
                                $('#modal-xl .modal-body .table_Common').DataTable().destroy();
                            }

                            table = $('#modal-xl .modal-body .table_Common').DataTable({
                                paging: true,
                                lengthChange: true,
                                searching: true,
                                ordering: true,
                                info: true,
                                autoWidth: true,
                                responsive: true,
                                pageLength: 10,
                                lengthMenu: [
                                    [10, 25, 50, -1],
                                    [10, 25, 50, 'All']
                                ],
                                //columnDefs: [
                                //	{ "targets": 0, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false },
                                //	{ "targets": -1, "className": "text-center", "width": "3%", "autoWidth": false, "searchable": false, "orderable": false }
                                //],
                                fixedColumns: true,
                                dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
                                    "<'row'<'col-sm-12'tr>>" +
                                    "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
                            });

                            $('#modal-xl .modal-body .table_Common').css('width', '100%');

                            table.on('click', 'tbody tr', function (e) {

                                $(e.currentTarget.parentElement).find('tr').each(function (i, ez) {
                                    if (i != (e.currentTarget.rowIndex - 1))
                                        $(ez).removeClass('bg-info');
                                });

                                //if (e.currentTarget.classList.contains('bg-info')) {

                                //}

                                e.currentTarget.classList.toggle('bg-info');

                            });


                        } catch { }

                        if ($('#modal-xl .modal-body .table_Common tbody tr').length > 0) {

                            $('#modal-xl').show(); //$('#modal-xl').modal('show');

                            var elem = document.createElement('div');
                            elem.className = "loader-overlay";
                            elem.style.cssText = 'position: fixed;top: 0; left: 0; z-index: 1045; width: 100vw; height: 100vh; background-color: rgb(0, 0, 0, .5);';
                            document.body.appendChild(elem);
                        }
                        else { CommonAlert_Error("No any record(s) found"); }
                    }
                    else {

                        $('#modal-xl').show();

                        var elem = document.createElement('div');
                        elem.className = "loader-overlay";
                        elem.style.cssText = 'position: fixed;top: 0; left: 0; z-index: 1045; width: 100vw; height: 100vh; background-color: rgb(0, 0, 0, .5);';
                        document.body.appendChild(elem);
                    }


                    try {

                        //Datemask dd/mm/yyyy

                        var locale_date_format = 'DD/MM/YYYY';

                        $('#modal-xl .modal-body .datepicker').each(function (i, e) {

                            $(e).daterangepicker({
                                singleDatePicker: true,
                                autoclose: true,
                                autoApply: true,
                                //timePicker: true,
                                //ranges: {
                                //    'Today': [moment(), moment()],
                                //    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                                //    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                                //    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                                //    'This Month': [moment().startOf('month'), moment().endOf('month')],
                                //    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                                //},
                                showDropdowns: true,
                                //maxDate: moment().format('DD/MM/YYYY'),
                                locale: { cancelLabel: 'Clear', format: locale_date_format/*, format: 'DD/MM/YYYY hh:mm A'*/ }
                            }, function (start, end, label) { try { fnChangeDatePicker(this, start, end, label); } catch { } });


                            var defaultDate = '';
                            var minDate = '';
                            var maxDate = '';

                            if (e.hasAttribute('data-max') && typeof e.getAttribute('data-max') != 'undefined' && e.getAttribute('data-max') != null && e.getAttribute('data-max').length > 0) {
                                maxDate = e.getAttribute('data-max');
                            }

                            if (e.hasAttribute('data-min') && typeof e.getAttribute('data-min') != 'undefined' && e.getAttribute('data-min') != null && e.getAttribute('data-min').length > 0) {
                                minDate = e.getAttribute('data-min');
                            }

                            if (e.hasAttribute('data-value') && typeof e.getAttribute('data-value') != 'undefined' && e.getAttribute('data-value') != null && e.getAttribute('data-value').length > 0) {
                                defaultDate = e.getAttribute('data-value');
                            }

                            $(e).val(defaultDate);

                            $(e).data('daterangepicker').maxDate = moment(maxDate, locale_date_format)
                            $(e).data('daterangepicker').minDate = moment(minDate, locale_date_format)

                            $(e).trigger('change');
                        })


                        $('#modal-xl .modal-body .select2').select2();

                    } catch { }

                    try { fnShow_Modal_Success($url, $title); } catch { }

                }, 1000);

            },
            failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
            error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
        });
}

function fnBackToList(url) {
    window.location.href = url;
}

function fnSubmitTable(objViewData, action, objData) {
    ShowLoader(true);


    try { localStorage.setItem('current-datatable-page', table_Common.page.info().page); } catch { }

    $.ajax({
        type: 'POST',
        url: action,
        data: { objViewData: objViewData, objData: objData },
        success: function (response) {

            ShowLoader(false);
            if (response.statusCode === 1) {
                Swal.fire({
                    icon: 'success',
                    title: response.message,
                    //type: "success"
                }).then(function () {
                    if (response.redirectURL != '' && response.redirectURL != null) {
                        window.location = response.redirectURL;
                    }
                    else {

                        fnFormDataSavedSuccess(response, table_Common)
                    }

                });
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: response.message,
                })
            }
        },
        failure: function (response) { ShowLoader(false); CommonAlert_Error(null) },
        error: function (response) { ShowLoader(false); CommonAlert_Error(null) }
    });
}

//function LoadCommonTable() {
//
//    if ($.fn.DataTable.isDataTable('#table_Policy_Master_List')) {
//        $('#table_Policy_Master_List').DataTable().destroy();
//    }


//    dtLoadPolicyList = $('#table_Policy_Master_List').DataTable({
//        "sAjaxSource": "@(IndexPageUrl.Replace("Index", ""))LoadPolicyList",
//        "language": {
//            "emptyTable": "No record found.",
//            "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw" style="color:#2a2b2b;"></i><span class="sr-only">Loading...</span>'
//        },
//        "initComplete": function (settings, json) { ShowLoader(false); loadDatatablePage('#table_Policy_Master_List'); },
//        "bServerSide": true,
//        "bProcessing": true,
//        "bSearchable": true,
//        "paging": true,
//        "lengthChange": true,
//        "searching": true,
//        "ordering": true,
//        "info": true,
//        "autoWidth": true,
//        "responsive": true,
//        "columnDefs": [{ "targets": 0, "className": "text-center", "width": "3%" }],
//        "fixedColumns": true,
//        "fnServerParams": function (aoData) { aoData.push({ "name": "From_Date", "value": $('#From_Date').val() }); aoData.push({ "name": "To_Date", "value": $('#To_Date').val() }); },
//        "fnCreatedRow": function (nRow, aData, iDataIndex) {
//            $('td:eq(0)', nRow).html('' + aData['rowNumDisplay']) == null ? "0" : aData['rowNumDisplay'];
//            $('td:eq(1)', nRow).html('' + aData['policy_No'] == null ? "" : aData['policy_No']);
//            $('td:eq(2)', nRow).html('' + aData['insured_Name'] == null ? "" : aData['insured_Name']);
//            $('td:eq(3)', nRow).html('' + aData['risk_Start_Date'] == null ? "" : aData['risk_Start_Date']);
//            $('td:eq(4)', nRow).html('' + aData['risk_End_Date'] == null ? "" : aData['risk_End_Date']);
//            $('td:eq(5)', nRow).html('' + aData['insurerName'] == null ? "" : aData['insurerName']);
//            $('td:eq(6)', nRow).html('' + aData['category_Subcategory'] == null ? "" : aData['category_Subcategory']);
//            $('td:eq(7)', nRow).html('' + aData['plan_Name'] == null ? "" : aData['plan_Name']);
//            $('td:eq(8)', nRow).html('' + aData['total_Sum_Assured'] == null ? "" : aData['total_Sum_Assured']);
//            $('td:eq(9)', nRow).html('' + aData['isActive'] == 'true' ? "Active" : '<span class="text-danger">' + 'InActive' + '</span>');
//            $('td:eq(10)', nRow).html('<div class="btn-group">' +
//                '<button type="button" class="btn btn-info btn-flat btn-sm" onclick="fnLoadParialViewForPolicyDetail(\'divForm_Add\', \'@Url.Action("AddEditPartial","PolicyMaster", new { Area = "Admin" })?id=' + aData['id'] + '\',' + aData['id'] + ')">' +
//                '<i class="fas fa-edit"></i></button>' +
//                '<button type="button" class="btn btn-info btn-sm ml-1" onclick="location.href=\'@Url.Action("GetDocuments","PolicyMaster", new { Area = "Admin" })?Id=' + aData['id'] + '\'">Documents</button>' +
//                '<button type="button" class="btn btn-info btn-sm ml-1" onclick="location.href=\'@Url.Action("EndorsementIndex","PolicyMaster", new { Area = "Admin" })?Policy_Detial_Id=' + aData['id'] + '\'">Endorsement</button></div>');
//            $(nRow).attr('id', 'tr_' + iDataIndex);
//        },
//        "columns": [
//            { "title": "#", "autoWidth": false, "searchable": false },
//            { "title": "Policy No.", "autoWidth": true, "searchable": true },
//            { "title": "Client Name", "autoWidth": true, "searchable": true },
//            { "title": "Risk Start Date", "autoWidth": true, "searchable": true },
//            { "title": "Risk End Date", "autoWidth": true, "searchable": true },
//            { "title": "Insurer Name", "autoWidth": true, "searchable": true },
//            { "title": "Category / Subcategory", "autoWidth": true, "searchable": true },
//            { "title": "Product Name", "autoWidth": true, "searchable": true },
//            { "title": "Total Sum Assured", "autoWidth": true, "searchable": true },
//            { "title": "Status", "autoWidth": false, "searchable": false },
//            { "title": "Action", "autoWidth": false, "searchable": false }
//        ],
//        //dom: 'Bfrtip',
//        dom: "<'row'<'col-sm-6'B>>" +
//            "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
//            "<'row'<'col-sm-12'tr>>" +
//            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
//        //buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
//        "buttons": [ //"excel",
//            {
//                extend: 'excelHtml5',
//                exportOptions: {
//                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9]
//                }
//            },
//            {
//                extend: 'excel',
//                exportOptions: {
//                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9]
//                }
//            },
//            {
//                extend: 'pdf',
//                exportOptions: {
//                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9]
//                }
//            },]//"buttons": ["csv", "excel", "pdf", "print"]
//    }).buttons().container().appendTo('#table_Policy_Master_List ' + '_wrapper .col-md-6:eq(0)');
//    // });
//    $.fn.DataTable.ext.errMode = 'none';

//}
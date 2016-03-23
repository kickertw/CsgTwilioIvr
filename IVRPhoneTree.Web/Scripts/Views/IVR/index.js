function loadCountryCodes() {
    $.ajax({
        cache: false,
        async: true,
        type: "POST",
        url: (data.Id ? "/QuestionLibrary/EditQuestion" : "/QuestionLibrary/CreateQuestion"),
        data: data
    }).done(function (data, status, jqXHR) {
        if (!data.Message) {
            $('div.submssion-success').removeClass('hidden');
            $('#visitQuestion').modal('hide');
            setTimeout(function () {
                $('div.submssion-success').fadeOut('slow', function () {
                    $(this).addClass('hidden');
                    $(this).attr('style', '');
                });
            }, 2000);
        } else {
            $('div.submssion-error').removeClass('hidden');
        }
    }).fail(function (data, status, jqXHR) {
        $('div.submssion-error').removeClass('hidden');
    });

}

$(document).ready(function () {
    
});
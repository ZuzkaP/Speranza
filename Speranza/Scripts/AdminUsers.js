$(function () {
    $('.IsAdmin').change(function () {

        var self = $(this);
        var id = self.attr('id');
        var value = self.prop('checked');
        $.ajax({
            url: "ToggleAdmin",
            data: { id: id, isAdmin: value },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 1)
                    $('#messageBox').html("<b>Zrušenie</b> administrátorských práv pre používateľa: <b>" + response.Email + "</b>");
                else if (response.Message == 2)
                    $('#messageBox').html("<b>Pridanie</b> administrátorských práv pre používateľa: <b>" + response.Email + "</b>");

                $('#messageBox').show();
            }
        });

    });
});

$(function () {
    $('.Category').change(function () {

        var self = $(this);
        var id = self.attr('id');
        var value = self.val();
        $.ajax({
            url: "UserCategory",
            data: { id: id, category: value },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 3)
                    $('#messageBox').html("Kategória pre používateľa <b>" + response.Email + "</b> bola zmenená na: <b>" + response.Category + "</b>");
                $('#messageBox').show();
            }
        });

    });
});

$(function () {
    $('.SignUpCount').click(function () {
        var self = $(this);
        var count = self.data("count");
        var email = self.data("email");
        $.ajax({
            url: "UpdateSignUpCount",
            data: { id: email, countUpdate: count },
            type: 'POST',
            dataType: "json",
            success: function (response) {
                if (response.Message == 4)
                    $('#messageBox').html("Počet vstupov užívateľovi <b>" + response.Email + "</b> bol zvýšený o <b>" + response.ChangeNumberOfSignUps + "</b>");
                else if (response.Message == 5) {
                    var changedCount = response.ChangeNumberOfSignUps;
                    if ($('#freeSignUpsCell-' + getEmailForJS(response.Email)).text() < response.ChangeNumberOfSignUps) {
                        changedCount = $('#freeSignUpsCell-' + getEmailForJS(response.Email)).text();
                    }
                    $('#messageBox').html("Počet vstupov užívateľovi <b>" + response.Email + "</b> bol znížený o <b>" + changedCount + "</b>");

                }

                $('#messageBox').show();
                $('#freeSignUpsCell-' + getEmailForJS(response.Email)).html(response.AfterChangeNumberOfSignUps);
            }
        });

    });
});


$(function () {
    $('.trainingsDetails').click(function () {
        var self = $(this);
        var email = self.data("email");
        $.ajax({
            url: "TrainingsDetails",
            data: { id: email },
            type: 'POST',
            dataType: "html",
            success: function (response) {
                $('.modal-body').html(response);
                $('#modalTitleEmail').html(email);
                $('#trainingDetailsModal').modal('show');
            }
        });

    });
});

$(document).ready(function () {
    $("#UsersTable").tablesorter({
        sortList: [[1, 0], [0, 0]],
        headers: {
            3: { sorter: false },
        },
        //textExtraction: function (node) {
        //    if (node.childNodes[0].tagName == "SELECT")
        //    {
        //        alert(node.childNodes[0]);
        //    }
        //    return node.innerHTML;
        //}
    });
    //$("#UsersTable").bind("sortStart", function () {
    //    .show();
    //}).bind("sortEnd", function () {
    //    .hide();
    //})

}
);

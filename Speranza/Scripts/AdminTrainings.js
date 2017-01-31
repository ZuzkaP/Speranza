$("#TrainingsTable").on("change", ".trainingDescription", function (event) {
    TrainingDescription($(this));
});

function TrainingDescription(element) {
    var trainingID = element.data('training');
    //value z textboxu
    var value = element.prop('value');
    $.ajax({
        url: "ChangeTrainingDescription",
        data: { trainingID: trainingID, description: value },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 1) {
                showMessageBox("Tréningu <b>" + $('#' + trainingID + "-date").html() + " " + $('#' + trainingID + "-time").html() + "</b> bol zmenený popis na: <b>" + value + "</b>");
            }

        }
    });
};


$("#TrainingsTable").on("change", ".trainingTrainer", function (event) {
    TrainingTrainer($(this));
});

function TrainingTrainer(element) {
    var trainingID = element.data('training');
    //value z textboxu
    var value = element.prop('value');
    $.ajax({
        url: "ChangeTrainer",
        data: { trainingID: trainingID, trainer: value },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 2) {
                showMessageBox("Tréningu <b>" + $('#' + trainingID + "-date").html() + " " + $('#' + trainingID + "-time").html() + "</b> bol priradený nový tréner: <b>" + value + "</b>");
            }

        }
    });

};

$("#TrainingsTable").on("change", ".trainingCapacity", function (event) {
    TrainingCapacity($(this));
});

function TrainingCapacity(element) {
    var trainingID = element.data('training');
    //value z textboxu
    var value = element.prop('value');
    $.ajax({
        url: "ChangeTrainingCapacity",
        data: { trainingID: trainingID, capacity: value },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 3) {
                showMessageBox("Kapacita tréningu <b>" + $('#' + trainingID + "-date").html() + " " + $('#' + trainingID + "-time").html() + "</b> bola zmenená na: <b>" + value + "</b>");
            }
            else if (response == 4) {
                showMessageBoxWarning("Kapacita tréningu nemôže byť menšia ako 1!");
            }
            else if (response == 13) {
                showMessageBoxWarning("Kapacita tréningu nemôže byť menšia ako počet prihlásených cvičiacich!");
                value++;
                element.val(value);
            }

        }
    });
}


$("#TrainingsTable").on("click", ".TrainingDetails", function (event) {
    TrainingDetails($(this));
});

function TrainingDetails(element) {

    var trainingID = element.data('training');
    $("#AddUser").val('');
    $("#messageBoxAddUserWarning").hide();
    $("#messageBoxAddUserSuccess").hide();
    $.ajax({
        //controller method
        url: "TrainingDetails",
        //controller method parameter : trainingID
        data: { trainingID: trainingID },
        type: 'POST',
        dataType: "html",
        success: function (response) {
            $('#TrainingDetailsBody').html(response);
            $('#modalTitleTraining').html($('#' + trainingID + "-date").html() + " " + $('#' + trainingID + "-time").html());
            $("#AddUser").data("training", trainingID);
            $('#trainingDetailsModal').modal('show');

        }
    });
}

$("#TrainingsTable").on("click", ".CancelTraining", function (event) {
    CancelTraining($(this));
});

function CancelTraining(changedElement) {
    var trainingID = changedElement.data('training');
    $.ajax({
        //controller method
        url: "CancelTraining",
        //controller method parameter : trainingID
        data: { trainingID: trainingID },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 10) {
                showMessageBoxWarning("Tréning <b>" + $('#' + trainingID + "-date").html() + " " + $('#' + trainingID + "-time").html() + "</b> bol zrušený!");
                $('#' + trainingID + '-Row').remove();
            }
        }
    })
}

$(function () {
    $('#AddNewTraining').click(function () {
        $('#TrainingDate').val('');
        $('#TrainingTime').val('');
        $('#NewTrainingTrainer').val('');
        $('#NewTrainingDescription').val('');
        $('#NewTrainingCapacity').val(10);
       
        $('#messageBoxNewTraining').hide();
        $('#NewTraining').modal('show');

    });
});

$("#NewTraining").keyup(function (event) {
    if (event.target.id != "NewTrainingDescription" && event.keyCode == 13) {
        $("#CreateTrainingButton").click();
    } CreateTrainingButton
});

$('#CreateTrainingButton').click(function () {
    var date = $('#TrainingDate').prop('value');
    var time = $('#TrainingTime').prop('value');
    var trainer = $('#NewTrainingTrainer').prop('value');
    var description = $('#NewTrainingDescription').val();
    var capacity = $('#NewTrainingCapacity').val();
    if (capacity == null || capacity == "") {
        capacity = -1;
    }
    $.ajax({
        //controller method
        url: "CreateNewTraining",
        //controller method parameter : trainingID
        data: { date: date, time: time, trainer: trainer, description: description, capacity: capacity },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            switch (response) {
                case 12:
                    $('#messageBoxNewTraining').html("Nie je možné vytvoriť tréning v minulosti!");
                    $('#messageBoxNewTraining').show();
                    return;
                case 5:
                    $('#messageBoxNewTraining').html("Je potrebné zadať aj trénera!");
                    $('#messageBoxNewTraining').show();
                    return;
                case 6:
                    $('#messageBoxNewTraining').html("Je potrebné zadať aj popis tréningu!");
                    $('#messageBoxNewTraining').show();
                    return;
                case 7:
                    $('#messageBoxNewTraining').html("Dátum je nesprávny!");
                    $('#messageBoxNewTraining').show();
                    return;
                case 8:
                    $('#messageBoxNewTraining').html("Čas je nesprávny!");
                    $('#messageBoxNewTraining').show();
                    return;
                case 4:
                    $('#messageBoxNewTraining').html("Kapacita tréningu nemôže byť menej ako 1!");
                    $('#messageBoxNewTraining').show();
                    return;
                default:
                    $('#NewTraining').modal('hide');

                    var newRow = '<tr id="' + response.TrainingID + '-Row">' +
                      '<td id="' + response.TrainingID + '-date">' + response.Date + '</td>' +
                      '<td id="' + response.TrainingID + '-time">' + response.Time + '</td>' +
                      '<td><textarea  class="trainingDescription form-control input-sm" rows="1" data-training="' + response.TrainingID + '">' + response.Description + '</textarea></td>' +
                      '<td><input type="text" value="' + response.Trainer + '" class="trainingTrainer form-control input-sm" data-training="' + response.TrainingID + '"/></td>' +
                      '<td>' +
                      '<span id="' + response.TrainingID + '-RegisteredNumber">0</span> ' +
                      '<span class="label label-info TrainingDetails" style="cursor:pointer" data-training="' + response.TrainingID + '">Detail</span>' +
                      '</td>' +
                      '<td><input type="number" min="1" value="' + response.Capacity + '" class="trainingCapacity form-control input-sm "data-training="' + response.TrainingID + '"/></td>' +
                      '<td><a class="CancelTraining" data-training="' + response.TrainingID + '" href="#">Zrušiť tréning</a></td>' +
                      '</tr>';

                    $('#TrainingsTable').prepend(newRow);
                    $("#messageBox").html("Nový tréning <b>" + response.Date + " " + response.Time + "</b> bol vytvorený!");
                    $("#messageBox").show();
            }
        }
    });
});


$(function () {
    $("#TrainingDate").datepicker({
        beforeShow: function () {
            setTimeout(function () {
                $('.ui-datepicker').css('z-index', 99999999999999);
            }, 0);
        },
        dateFormat: "dd.mm.yy",
        minDate: 0,
        constrainInput: true,
        monthNames: ["Január", "Február", "Marec", "Apríl", "Máj", "Jún", "Júl", "August", "September", "Október", "November", "December"],
        monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "Máj", "Jún", "Júl", "Aug", "Sep", "Okt", "Nov", "Dec"],
        dayNames: ["Nedeľa", "Pondelok", "Utorok", "Streda", "Štvrtok", "Piatok", "Sobota"],
        dayNamesMin: ["Ne", "Po", "Ut", "St", "Št", "Pi", "So"],
        firstDay: 1
    });
});


$('#TrainingTime').timepicker(
    {
        showPeriodLabels: false,
        showOn: 'both',
        button: '#trainingTimeButton',
        hourText: 'Hodiny',
        minuteText: 'Minúty',
        minutes: {
            interval: 15
        }
    }
  );

$('#SignOffLimit').change(function () {
    var value = $(this).val();
    if (value == 0 || value >= 5) {
        $("#SignOffLimitHours").html('hodín');
    }
    else if (value == 1) {
        $("#SignOffLimitHours").html('hodina');
    }
    else {
        $("#SignOffLimitHours").html('hodiny');
    }
    $.ajax({
        //controller method
        url: "SetSignOffLimit",
        //controller method parameter : trainingID
        data: { limit: value },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 14) {
                showMessageBox("Limit na odhlásenie z tréningu sa zmenil.");
            }
        }
    })
});

function showMessageBox(text) {
    $('#messageBoxWarning').hide();
    $("#messageBox").html(text);
    $("#messageBox").show();
}

function showMessageBoxWarning(text) {
    $('#messageBox').hide();
    $("#messageBoxWarning").html(text);
    $("#messageBoxWarning").show();
}

$(document).ready(function () {
    $("#TrainingsTable").tablesorter({
        headers: {
            6: { sorter: false },
            2: { sorter: false },
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

});

$("#AddUserButton").click(function () {
    var trainingID = $("#AddUser").data("training");
    var user = $("#AddUser").val();
    $.ajax({
        //controller method
        url: "AddUserToTraining",
        //controller method parameter : trainingID
        data: { trainingID: trainingID, userData:user },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            
            if (response == 4) {
                showMessageBoxWarningForAddingUser(user + " už je prihlásený na tento tréning.");
            }
            else if (response == 6) {
                showMessageBoxWarningForAddingUser("Daný používateľ neexistuje.");
            }
            else if (response == 1) {
                showMessageBoxWarningForAddingUser("Tréning neexistuje.");
            }
            else if (response == 2) {
                showMessageBoxWarningForAddingUser("Tréning má už naplnenú kapacitu.");
            }
            else
            {
                showMessageBoxSuccessForAddingUser(response.Name + " " + response.Surname + " bol prihlásený na tento tréning.");
                if ($('#UsersInTrainingTable').val() == undefined)
                {
                    
                    var newTable = '<table class="table table-striped table-hover" id="UsersInTrainingTable">' +
                                    ' <thead>' +
                                    '  <tr>' +
                                    '<th>Meno</th>' +
                                    '<th>Priezvisko</th>' +
                                   ' <th>Email</th>' +
                                    '<th>Akcia</th>' +
                               ' </tr>' +
                                '</thead>' +
                     ' </table>';
                    $("#TrainingDetailsBody").html(newTable);
                }

                    var newRow = '<tr id="' + response.Email + '">' +
                         '<td>' + response.Name + '</td>' +
                         '<td>' + response.Surname + '</td>' +
                         '<td>' + response.Email + '</td>' +
                         '<td><a class="UserSignOutFromTraining" data-email="' + response.Email + '" data-training="' + trainingID + '" href="#">Odhlás používateľa</a></td>' +
                         '</tr>';

                    $('#UsersInTrainingTable').prepend(newRow);
                    $('#' + trainingID + '-RegisteredNumber').html(1+parseInt($('#' + trainingID + '-RegisteredNumber').html(),10));
            }
            
        }
    })
})


$("#TrainingDetailsBody").on("click", ".UserSignOutFromTraining", function (event) {
    SignOutFromTraining($(this));
});

function SignOutFromTraining(changedElement) {
    {
            var id = changedElement.data('email');
            var training = changedElement.data('training');
            $.ajax({
                url: "../AdminUsers/SignOutFromTraining" ,
                data: { id: id, training: training },
                type: 'POST',
                dataType: "json",
                success: function (response) {
                    if (response.Message == 6) {
                        $("#messageBoxAddUserWarning").hide();
                        $('#' + id).remove();
                        $('#messageBoxAddUserSuccess').html("Užívateľ <b>" + response.Email + "</b> bol úspešne odhlásený z tréningu <b>" + response.TrainingDate + "</b> o: <b>" + response.TrainingTime + "</b>!");
                        $('#messageBoxAddUserSuccess').show();
                        $('#' + training + '-RegisteredNumber').html($('#' + training + '-RegisteredNumber').html()-1);
                    }
                }
            });

        }
    };


function showMessageBoxSuccessForAddingUser(text) {
    $('#messageBoxAddUserWarning').hide();
    $("#messageBoxAddUserSuccess").html(text);
    $("#messageBoxAddUserSuccess").show();
}

function showMessageBoxWarningForAddingUser(text) {
    $('#messageBoxAddUserSuccess').hide();
    $("#messageBoxAddUserWarning").html(text);
    $("#messageBoxAddUserWarning").show();
}
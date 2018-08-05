$(".ConfirmParticipation").click(function () {
    var self = $(this);
    var trainingID = self.data("training");
    var email = self.data("email");
    $.ajax({
        //controller method
        url: "ConfirmParticipation",
        //controller method parameter : trainingID
        data: { trainingID: trainingID, email: email },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 15) {
                $("#cp-" + email.replace("@", "")).hide();
                $("#dp-" + email.replace("@", "")).hide();
                $("#messageBoxParticipation").html("Účasť bola potvrdená!");
                $("#messageBoxParticipation").show();
            }
        }
    });
});

$(".DisproveParticipation").click(function () {
    var self = $(this);
    var trainingID = self.data("training");
    var email = self.data("email");
    $.ajax({
        //controller method
        url: "DisproveParticipation",
        //controller method parameter : trainingID
        data: { trainingID: trainingID, email: email },
        type: 'POST',
        dataType: "json",
        success: function (response) {
            if (response == 16) {
                $("#cp-" + email.replace("@", "")).hide();
                $("#dp-" + email.replace("@", "")).hide();
                $("#messageBoxParticipation").html("Neúčasť bola potvrdená!");
                $("#messageBoxParticipation").show();
                $("#Participation").html("Nezúčastnil sa");
            }
        }
    });
});
    var cssTicked = "glyphicon glyphicon-ok-sign cellicon";
    var cssNotTicked = "glyphicon glyphicon-one-fine-empty-dot cellicon";
    var questionTypeAgreeDisagree = 1;
    var questionTypeTextBox = 2;
    var questionTypeYesNoNA = 3;
    var cssStatusOK_Colour = "text-success";
    var cssStatusOK_Icon = "glyphicon glyphicon-ok-circle";
    var cssStatusWarning_Colour = "cellicon";
    var cssStatusWarning_Icon = "glyphicon glyphicon-certificate";
    var cssStatusDanger_Colour = "text-danger";
    var cssStatusDanger_Icon = "glyphicon glyphicon-certificate";
    var answerIdYes= 6;
    var answerIdNo = 7;
    var answerIdNA = 8;
    var classificationIdNA = 5;

    function tickOff(targetObj) {
        document.getElementById(targetObj).className = cssNotTicked;
    }

    function tickOn(targetObj) {
        document.getElementById(targetObj).className = cssTicked;
    }
            
    function changeStatus(answerID, questionType, targetObjColor, targetObjIcon) {
        // show the status lights - red orange green
        var statusIcon = "";
        var statusColor = "";
        switch (questionType) {
            case questionTypeYesNoNA:
                switch (answerID) {
                    case answerIdYes:
                        statusColor = cssStatusOK_Colour;
                        statusIcon = cssStatusOK_Icon;
                        break;
                    case answerIdNo:
                        statusColor = cssStatusDanger_Colour;
                        statusIcon = cssStatusDanger_Icon;
                        break;
                    case answerIdNA:
                        statusColor = cssStatusWarning_Colour;
                        statusIcon = cssStatusWarning_Icon;
                        break;
                }
                break;
        }

        // update the css according to the answer provided..
        document.getElementById(targetObjColor).className = statusColor;
        document.getElementById(targetObjIcon).className = statusIcon;
                 
    }

    function tickMe(questionID, answerID, rowIndex, questionType) {
        // controls are: ContentPlaceHolder1_rptQandA_lblAnswer<answerid>_<index>

        // first of all, switch off all ticks..
        switch (questionType) {
            case questionTypeYesNoNA:
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer6_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer7_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer8_" + rowIndex);
                break;

            case questionTypeAgreeDisagree:
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer1_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer2_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer3_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer4_" + rowIndex);
                tickOff("ContentPlaceHolder1_rptQandA_lblAnswer5_" + rowIndex);
                break;
        }
                 
        // now tick the chosen answer
        tickOn("ContentPlaceHolder1_rptQandA_lblAnswer" + answerID + "_" + rowIndex);

        // change the status for this question - red orange green
        //     Color: ContentPlaceHolder1_rptQandA_pnlStatus_<index>
        //     Icon: ContentPlaceHolder1_rptQandA_lblStatus_<index>
        changeStatus(answerID, questionType, "ContentPlaceHolder1_rptQandA_pnlStatus_" + rowIndex, "ContentPlaceHolder1_rptQandA_lblStatus_" + rowIndex)

        // when the user selects YES or NO, we automatically open the more details 
        //   but when they select N/A we don't and we auto-set the classification dropdown to N/A also
        //      overwrite the current setting for classification too
        switch (answerID) {
            case answerIdYes:
            case answerIdNo:
                showMore(rowIndex);
                break;

            case answerIdNA:
                $('#ContentPlaceHolder1_rptQandA_cboClassification_' + rowIndex).val(classificationIdNA);
                break;
        }
        
        // we have now responded instantly to the user so we can now go to the database and update the Q&A
        ajaxCall(questionID, answerID, 'null', 'null');
    }

    function toggleMore(rowIndex) {
        // rowMoreInfo_<index>
        $("#rowMoreInfo_" + rowIndex).toggle();
    }

    function showMore(rowIndex) {
        // rowMoreInfo_<index>
        $("#rowMoreInfo_" + rowIndex).show();
    }

    function showOne(targetObj) {
        $("#" + targetObj).show();
    }
                
    function showAll() {
        $(".moreinfo").show();
    }

    function hideAll() {
        $(".moreinfo").hide();
    }

    function saveObservations(questionID, controlRef) {
        // get the current text from the box
        var oControl = document.getElementById(controlRef);
        var answerText = oControl.value;

        ajaxCall(questionID, 0, answerText, 'null');
    }

    function saveClassification(questionID, controlRef) {
        // get the current text from the box
        var oControl = document.getElementById(controlRef);
        var selectedAnswerId = oControl.options[oControl.selectedIndex].value;

        ajaxCall(questionID, 0, 'null', selectedAnswerId);
    }
        
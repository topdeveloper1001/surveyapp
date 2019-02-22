using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyApp
{
    public class cConstants
    {
        // audit-trail events & also useful for push notifications
        public const int eventUserLogin = 1;
        public const int eventUserLogout = 2;
        public const int eventUserLoginFailed = 3;
        public const int eventClientNew = 4;
        public const int eventClientEdit = 5;
        public const int eventClientDelete = 6;
        public const int eventLocationNew = 7;
        public const int eventLocationEdit = 8;
        public const int eventLocationDelete = 9;
        public const int eventUserNew = 10;
        public const int eventUserEdit = 11;
        public const int eventUserDelete = 12;
        public const int eventSurveySummaryNew = 13;
        public const int eventSurveySummaryEdit = 14;
        public const int eventSurveyDelete = 15;
        public const int eventQuestionSave = 16;
        public const int eventQuestionSubmitToHQ = 17;
        public const int eventQuestionSubmitToHQFail = 18;
        public const int eventFileUpload = 19;
        public const int eventFileDelete = 20;
        public const int eventClientApprovalAccept = 21;
        public const int eventClientApprovalReject = 22;
        public const int eventClientResponseSave = 23;
        public const int eventClientResponseClose = 24;
        public const int eventSurveyConsultantChanged = 25;
        public const int eventSurveyStatusReadyForClientReview = 26;

        public const int eventClientResponsesClosed_NotifyClientMgr = 27;
        public const int eventClientResponsesClosed_NotifyClientAdmin = 28;
        public const int eventSurveyConsultantChanged_Notification = 29;
        public const int eventSurveyStatusReadyForClientReview_Notification = 30;
        public const int eventQuestionSubmitToHQ_Notification = 31;
        public const int eventClientApprovalAccept_Notification = 32;
        public const int eventClientApprovalReject_Notification = 33;

        //audit trail
        //m_data.auditEvent(cConstants.event


        public const string cssNotTicked = "glyphicon glyphicon-one-fine-empty-dot cellicon";
        public const string cssTicked = "glyphicon glyphicon-ok-sign cellicon";

        public const int questionTypeAgreeDisagree = 1;
        public const int questionTypeTextBox = 2;
        public const int questionTypeYesNoNA = 3;

        public const int answerIdYes= 6;
        public const int answerIdNo = 7;
        public const int answerIdNA = 8;

        public const string cssStatusOK_Colour = "text-success";
        public const string cssStatusOK_Icon = "glyphicon glyphicon-ok-circle";
        public const string cssStatusWarning_Colour = "cellicon";
        public const string cssStatusWarning_Icon = "glyphicon glyphicon-certificate";
        public const string cssStatusDanger_Colour = "text-danger";
        public const string cssStatusDanger_Icon = "glyphicon glyphicon-certificate";

        public const int statusDraft = 1;
        public const int statusForInternalReview = 2;
        public const int statusForClientReview = 3;
        public const int statusReleased = 4;

        // sources for file uploads so we can separate out the auditor from the client files
        public const int sourceAuditor = 1;
        public const int sourceClient = 2;

        public const int approvalStatus_Pending = 1;
        public const int approvalStatus_Approved = 2;
        public const int approvalStatus_Rejected = 3;

        public const string msgAuditRejected = "Your request has been submitted";
    }
}

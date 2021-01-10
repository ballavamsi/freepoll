/*
SQLyog Community v13.1.5  (64 bit)
MySQL - 8.0.13-4 : Database - tEV5KSfnfS
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`tEV5KSfnfS` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;

USE `tEV5KSfnfS`;

/*Table structure for table `Data_StarOptions` */

DROP TABLE IF EXISTS `Data_StarOptions`;

CREATE TABLE `Data_StarOptions` (
  `star_option_id` int(11) NOT NULL AUTO_INCREMENT,
  `display_order` int(11) DEFAULT '1',
  `is_active` int(11) DEFAULT '1',
  `option_display_text` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `option_text` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`star_option_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Data_StarOptions` */

insert  into `Data_StarOptions`(`star_option_id`,`display_order`,`is_active`,`option_display_text`,`option_text`) values 
(1,1,1,'Very Good|Good|Average|Bad|Very Bad','Very Good|Good|Average|Bad|Very Bad'),
(2,1,1,'Strongly Agree|Agree|Neutral|Disagree|Strongly Disagree','Strongly Agree|Agree|Neutral|Disagree|Strongly Disagree'),
(3,1,1,'Poor|Fair|Good|Very Good|Exceptional','Poor|Fair|Good|Very Good|Exceptional');

/*Table structure for table `Poll` */

DROP TABLE IF EXISTS `Poll`;

CREATE TABLE `Poll` (
  `poll_id` int(11) NOT NULL AUTO_INCREMENT,
  `created_by` int(11) DEFAULT NULL,
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `duplicate` int(10) unsigned zerofill DEFAULT NULL,
  `enddate` date DEFAULT NULL,
  `name` varchar(1000) COLLATE utf8_unicode_ci NOT NULL,
  `poll_guid` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status_id` int(11) DEFAULT NULL,
  `type` int(1) unsigned zerofill DEFAULT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`poll_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Poll` */

/*Table structure for table `Poll_Options` */

DROP TABLE IF EXISTS `Poll_Options`;

CREATE TABLE `Poll_Options` (
  `poll_option_id` int(11) NOT NULL AUTO_INCREMENT,
  `poll_id` int(11) DEFAULT NULL,
  `created_by` int(11) DEFAULT NULL,
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `option_text` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `order_id` int(11) DEFAULT NULL,
  `status_id` int(11) DEFAULT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`poll_option_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Poll_Options` */

/*Table structure for table `Poll_Votes` */

DROP TABLE IF EXISTS `Poll_Votes`;

CREATE TABLE `Poll_Votes` (
  `poll_vote_id` int(11) NOT NULL AUTO_INCREMENT,
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `ip_address` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `option_id` int(11) DEFAULT NULL,
  `poll_id` int(11) DEFAULT NULL,
  `user_location` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`poll_vote_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Poll_Votes` */

/*Table structure for table `Question_Type` */

DROP TABLE IF EXISTS `Question_Type`;

CREATE TABLE `Question_Type` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `display_order` int(11) DEFAULT NULL,
  `is_active` int(1) unsigned zerofill DEFAULT '1',
  `type_code` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type_value` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Question_Type` */

insert  into `Question_Type`(`type_id`,`display_order`,`is_active`,`type_code`,`type_value`) values 
(1,1,0,'essay','Essay'),
(2,2,1,'radiobuttons','Single Option'),
(3,3,1,'imageradiobuttons','Single Image Option'),
(4,4,1,'multiple','Multiple Options'),
(5,5,1,'imagemultiple','Multiple Image Options'),
(6,6,1,'customrating','Custom Ratings'),
(7,10,1,'multiplerating','Multiple Star Ratings'),
(8,8,1,'rangeslider','Range Slider'),
(9,7,1,'slider','Slider'),
(10,9,1,'starrating','Star');

/*Table structure for table `Status` */

DROP TABLE IF EXISTS `Status`;

CREATE TABLE `Status` (
  `statusid` int(11) DEFAULT NULL,
  `statusname` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Status` */

insert  into `Status`(`statusid`,`statusname`) values 
(1,'UnPublished'),
(2,'Published'),
(3,'Deleted'),
(0,'New');

/*Table structure for table `Survey` */

DROP TABLE IF EXISTS `Survey`;

CREATE TABLE `Survey` (
  `surveyid` int(11) NOT NULL AUTO_INCREMENT,
  `allowduplicate` int(1) DEFAULT '0',
  `created_by` int(11) DEFAULT NULL,
  `emailidrequired` int(1) DEFAULT '0',
  `enableprevious` int(1) DEFAULT '0',
  `enddate` date DEFAULT NULL,
  `endtitle` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status_id` int(11) DEFAULT NULL,
  `survey_guid` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `welcomedescription` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `welcomeimage` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `welcometitle` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `askemail` int(11) DEFAULT '0',
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_date` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`surveyid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Survey` */

/*Table structure for table `Survey_Feedback` */

DROP TABLE IF EXISTS `Survey_Feedback`;

CREATE TABLE `Survey_Feedback` (
  `survey_feedback_id` int(11) NOT NULL AUTO_INCREMENT,
  `completed_datetime` datetime DEFAULT NULL,
  `inserted_datetime` datetime DEFAULT CURRENT_TIMESTAMP,
  `review_comment` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `review_completed` int(1) DEFAULT '0',
  `review_datetime` datetime DEFAULT NULL,
  `survey_id` int(11) DEFAULT NULL,
  `survey_user_email` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `survey_user_guid` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`survey_feedback_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Survey_Feedback` */

/*Table structure for table `Survey_Feedback_Question_Options` */

DROP TABLE IF EXISTS `Survey_Feedback_Question_Options`;

CREATE TABLE `Survey_Feedback_Question_Options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `custom_answer` varchar(10000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `inserted_datetime` datetime DEFAULT CURRENT_TIMESTAMP,
  `survey_feedback_id` int(11) DEFAULT NULL,
  `survey_question_id` int(11) DEFAULT NULL,
  `survey_question_option_id` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Survey_Feedback_Question_Options` */

/*Table structure for table `Survey_Question_Options` */

DROP TABLE IF EXISTS `Survey_Question_Options`;

CREATE TABLE `Survey_Question_Options` (
  `survey_question_option_id` int(11) NOT NULL AUTO_INCREMENT,
  `created_by` int(11) DEFAULT NULL,
  `display_order` int(11) DEFAULT NULL,
  `option_key` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `option_value` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `survey_question_id` int(11) DEFAULT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`survey_question_option_id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Survey_Question_Options` */

/*Table structure for table `Survey_Questions` */

DROP TABLE IF EXISTS `Survey_Questions`;

CREATE TABLE `Survey_Questions` (
  `survey_question_id` int(11) NOT NULL AUTO_INCREMENT,
  `created_by` int(11) DEFAULT NULL,
  `created_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `isrequired` int(1) DEFAULT '0',
  `question_display_order` int(11) DEFAULT NULL,
  `status_id` int(11) DEFAULT NULL,
  `subtitle` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `survey_id` int(11) DEFAULT NULL,
  `title` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type_id` int(11) DEFAULT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `updated_date` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`survey_question_id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `Survey_Questions` */

/*Table structure for table `User` */

DROP TABLE IF EXISTS `User`;

CREATE TABLE `User` (
  `userid` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `email` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `github` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `google` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `facebook` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `user_guid` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` int(11) DEFAULT NULL,
  `photo_url` varchar(1000) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

/*Data for the table `User` */

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

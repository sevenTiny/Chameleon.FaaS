/*
Navicat MySQL Data Transfer

Source Server         : 47.95.210.212
Source Server Version : 50642
Source Host           : 47.95.210.212:39901
Source Database       : SevenTinyCloudFaaS

Target Server Type    : MYSQL
Target Server Version : 50642
File Encoding         : 65001

Date: 2019-10-25 19:42:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for FaaS_Settings
-- ----------------------------
DROP TABLE IF EXISTS `FaaS_Settings`;
CREATE TABLE `FaaS_Settings` (
  `IsDebugMode` int(11) NOT NULL COMMENT '是否Debug编译',
  `IsOutPutFiles` int(11) NOT NULL,
  `ReferenceDirs` varchar(255) DEFAULT NULL COMMENT '要引用的dll的路径'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of FaaS_Settings
-- ----------------------------
INSERT INTO `FaaS_Settings` VALUES ('0', '0', 'C:\\Users\\wangdong3\\.nuget\\packages,C:\\Users\\7tiny\\.nuget\\packages,C:\\Users\\Lhp13\\.nuget\\packages');

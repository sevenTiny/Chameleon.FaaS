/*
Navicat MySQL Data Transfer

Source Server         : 47.95.210.212
Source Server Version : 50642
Source Host           : 47.95.210.212:39901
Source Database       : SevenTinyCloudFaaS

Target Server Type    : MYSQL
Target Server Version : 50642
File Encoding         : 65001

Date: 2019-10-25 19:42:20
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Logging
-- ----------------------------
DROP TABLE IF EXISTS `Logging`;
CREATE TABLE `Logging` (
  `Group` varchar(255) NOT NULL COMMENT '组，判断当前程序集使用的是哪个组的配置信息，如果没有找到对应组的配置信息，则采用root（默认）组的配置信息，root组的配置信息是不允许随意改动和删除的',
  `Level_Info` int(11) NOT NULL,
  `Level_Debug` int(11) NOT NULL,
  `Level_Warn` int(11) NOT NULL,
  `Level_Error` int(11) NOT NULL,
  `Level_Fatal` int(11) NOT NULL,
  `Directory` varchar(255) DEFAULT '' COMMENT '日志文件的路径，默认空（程序集的根目录）',
  PRIMARY KEY (`Group`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of Logging
-- ----------------------------
INSERT INTO `Logging` VALUES ('Default', '0', '1', '0', '1', '0', '');

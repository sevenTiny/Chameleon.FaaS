/*
Navicat MySQL Data Transfer

Source Server         : 47.95.210.212
Source Server Version : 50642
Source Host           : 47.95.210.212:39901
Source Database       : SevenTinyCloudFaaS

Target Server Type    : MYSQL
Target Server Version : 50642
File Encoding         : 65001

Date: 2019-10-25 19:42:05
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for FaaS_CSharpAssemblyReference
-- ----------------------------
DROP TABLE IF EXISTS `FaaS_CSharpAssemblyReference`;
CREATE TABLE `FaaS_CSharpAssemblyReference` (
  `AppName` varchar(255) NOT NULL,
  `Assembly` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of FaaS_CSharpAssemblyReference
-- ----------------------------
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'newtonsoft.json\\12.0.2\\lib\\netstandard2.0\\Newtonsoft.Json.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'SevenTiny.Cloud.MultiTenantPlatform.UI.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'SevenTiny.Cloud.Infrastructure.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'mongodb.bson\\2.8.1\\lib\\netstandard1.5\\MongoDB.Bson.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'mongodb.driver\\2.8.1\\lib\\netstandard1.5\\MongoDB.Driver.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'mongodb.driver.core\\2.8.1\\lib\\netstandard1.5\\MongoDB.Driver.Core.dll');
INSERT INTO `FaaS_CSharpAssemblyReference` VALUES ('System', 'seventiny.bantina.logging\\1.0.17\\lib\\netstandard2.0\\SevenTiny.Bantina.Logging.dll');

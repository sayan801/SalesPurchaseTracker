-- MySQL dump 10.13  Distrib 5.5.9, for Win32 (x86)
--
-- Host: localhost    Database: sptdb
-- ------------------------------------------------------
-- Server version	5.5.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `sptdb`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `sptdb` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `sptdb`;

--
-- Table structure for table `customer_payment`
--

DROP TABLE IF EXISTS `customer_payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customer_payment` (
  `customer_id` varchar(45) NOT NULL,
  `payment_amount` double NOT NULL,
  `payment_date` datetime NOT NULL,
  `payment_id` varchar(45) NOT NULL,
  PRIMARY KEY (`payment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_payment`
--

LOCK TABLES `customer_payment` WRITE;
/*!40000 ALTER TABLE `customer_payment` DISABLE KEYS */;
INSERT INTO `customer_payment` VALUES ('40831.48671303241',9000,'2011-10-15 11:42:10','payment-40831.4876252662'),('40831.48717956023',8990,'2011-10-15 11:44:07','payment-40831.4889719444'),('40831.48615660880',0,'2011-10-15 13:12:31','payment-40831.5503617245'),('40831.4869148382',100,'2011-10-15 13:13:00','payment-40831.5506982407'),('40831.48671303241',60,'2011-10-22 15:07:14','payment-40838.6300302199'),('40831.48615660880',1000,'2011-10-23 14:12:25','payment-40839.5919575347'),('40831.48671303241',1000,'2011-10-23 14:48:53','payment-40839.6172805556'),('40831.48615660880',1000,'2011-10-23 14:57:32','payment-40839.6232884491'),('40831.48615660880',1001.135,'2011-10-23 14:58:46','payment-40839.6241502431'),('40831.48615660880',800,'2011-10-23 22:00:53','payment-40839.9172876736'),('40831.48671303241',0,'2012-04-18 19:02:42','payment-41017.7935518866'),('40831.48671303241',0,'2012-04-18 19:04:39','payment-41017.7948973843'),('40831.48671303241',1200,'2012-05-26 19:15:29','payment-41055.8024256713'),('40831.48671303241',10002,'2012-05-26 19:33:10','payment-41055.8147101042'),('40831.48615660880',10025,'2012-05-26 19:41:14','payment-41055.820309294'),('40831.4869148382',4555,'2012-05-26 19:45:41','payment-41055.8233943056');
/*!40000 ALTER TABLE `customer_payment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customers` (
  `sl_no` int(11) DEFAULT NULL,
  `id` varchar(45) NOT NULL,
  `customer_name` varchar(70) NOT NULL,
  `address` varchar(70) DEFAULT NULL,
  `ph_no` varchar(45) DEFAULT '(+91)',
  `vat_no` varchar(45) DEFAULT NULL,
  `turn_over` double DEFAULT NULL,
  `due` double DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (NULL,'40831.48615660880','pulak','barasat','132453663737','ghdjdui890',5983.135,-6843),(NULL,'40831.48671303241','prasun','bhagar','6272782299089','sdghdyu789',8188,-13074),(NULL,'40831.4869148382','tamal','USA','42526722728282','gdhdyu789',125.84,-4429.16);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchasebilling`
--

DROP TABLE IF EXISTS `purchasebilling`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `purchasebilling` (
  `product` varchar(45) NOT NULL,
  `quantity` double NOT NULL,
  `vat` double NOT NULL,
  `rate` double NOT NULL,
  `amount` double NOT NULL,
  `invoiceNo` varchar(45) NOT NULL,
  `billItemId` varchar(45) NOT NULL,
  PRIMARY KEY (`billItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchasebilling`
--

LOCK TABLES `purchasebilling` WRITE;
/*!40000 ALTER TABLE `purchasebilling` DISABLE KEYS */;
INSERT INTO `purchasebilling` VALUES ('pen',10,0,20,200,'JGB-IN-PURCS-0001-40831.523525706','JGB-IN-PURCS-0001-40831.523525706_1'),('pen',20,4,40,800,'JGB-IN-PURCS-0001-40831.5572335185','JGB-IN-PURCS-0001-40831.5572335185_1'),('biscuit',10,13.5,100,1000,'JGB-IN-PURCS-0003-40831.4819798495','JGB-IN-PURCS-0003-40831.4819798495_1'),('pen',10,4,10,100,'JGB-IN-PURCS-0003-40831.4819798495','JGB-IN-PURCS-0003-40831.4819798495_2'),('biscuit',22,13.5,200,4400,'JGBP-0001-40838.4785145139','JGBP-0001-40838.4785145139_1'),('biscuit',1,0,10,10,'JGBP-0001-40839.8545471875','JGBP-0001-40839.8545471875_1'),('bish',10,4,190,1900,'JGBP-0001-40839.898515625','JGBP-0001-40839.898515625_1'),('biscuit',1,0,10,10,'JGBP-0002-40839.8459304861','JGBP-0002-40839.8459304861_1'),('ak47',10,4,1000,10000,'JGBP-0002-40839.8570994444','JGBP-0002-40839.8570994444_1'),('lenovo',10,4,30,300,'JGBP-0002-41017.793913831','JGBP-0002-41017.793913831_1'),('ak47',10,4,2000,20000,'JGBP-0003-40839.8580063889','JGBP-0003-40839.8580063889_1');
/*!40000 ALTER TABLE `purchasebilling` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchaselist`
--

DROP TABLE IF EXISTS `purchaselist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `purchaselist` (
  `invoiceNo` varchar(45) NOT NULL,
  `vendorId` varchar(45) DEFAULT NULL,
  `datePurchase` datetime DEFAULT NULL,
  `totalAmount` double DEFAULT NULL,
  `payment` double DEFAULT NULL,
  `vendorName` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`invoiceNo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchaselist`
--

LOCK TABLES `purchaselist` WRITE;
/*!40000 ALTER TABLE `purchaselist` DISABLE KEYS */;
INSERT INTO `purchaselist` VALUES ('JGBP-0001-40838.4785145139','Vendor-1-40831.5295948727','2011-10-22 11:29:40',4400,900,'SUMAN NANDI'),('JGBP-0001-40839.8545471875','Vendor-1-40831.5295948727','2011-10-23 20:31:00',10,510,'SUMAN NANDI'),('JGBP-0001-40839.898515625','Vendor-1-40831.5295948727','2011-10-23 21:34:16',1900,1900,'SUMAN NANDI'),('JGBP-0002-40839.8459304861','Vendor-1-40831.5295948727','2011-10-23 20:18:40',10,1000,'SUMAN NANDI'),('JGBP-0002-40839.8570994444','Vendor-1-40831.5295948727','2011-10-23 20:35:18',10000,10400,'SUMAN NANDI'),('JGBP-0002-41017.793913831','Vendor-1-40831.4780250347','2012-04-18 19:03:36',300,0,'anirban'),('JGBP-0003-40839.8580063889','Vendor-2-40831.4782351852','2011-10-23 20:36:09',20000,20800,'indra');
/*!40000 ALTER TABLE `purchaselist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `salesbilling`
--

DROP TABLE IF EXISTS `salesbilling`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `salesbilling` (
  `description` varchar(50) NOT NULL,
  `quantity` double NOT NULL,
  `vat` double NOT NULL,
  `rate` double NOT NULL,
  `amount` double NOT NULL,
  `invoiceNo` varchar(45) NOT NULL,
  `billItemId` varchar(90) NOT NULL,
  PRIMARY KEY (`billItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `salesbilling`
--

LOCK TABLES `salesbilling` WRITE;
/*!40000 ALTER TABLE `salesbilling` DISABLE KEYS */;
INSERT INTO `salesbilling` VALUES ('biscuit',22,13.5,200,4400,'JGB-IN-SALES-0001-40831.5501628009','JGB-IN-SALES-0001-40831.5501628009_1'),('novice games',11,4,11,121,'JGB-IN-SALES-0002-40831.5503839468','JGB-IN-SALES-0002-40831.5503839468_1'),('chowmin',10,4,120,1200,'JGB-IN-SALES-0005-40831.4873233333','JGB-IN-SALES-0005-40831.4873233333_1'),('pen',10,0,20,200,'JGB-IN-SALES-0005-40831.4873233333','JGB-IN-SALES-0005-40831.4873233333_2'),('chowmin',10,4,120,1200,'JGB-IN-SALES-0006-40831.4877921759','JGB-IN-SALES-0006-40831.4877921759_1'),('biscuit',20,13.5,200,4000,'JGB-IN-SALES-0006-40831.4877921759','JGB-IN-SALES-0006-40831.4877921759_2'),('pen',40,0,80,3200,'JGB0001-40838.6296847107','JGB0001-40838.6296847107_1'),('biscuit',1,0,4,4,'JGB0001-40839.6169587153','JGB0001-40839.6169587153_1'),('chow',1,13.5,1,1,'JGB0001-40839.6235526736','JGB0001-40839.6235526736_1'),('bish',5,4,190,950,'JGB0001-40839.9170400463','JGB0001-40839.9170400463_1'),('hello',10,4,230,2300,'JGB0001-41017.791994213','JGB0001-41017.791994213_1'),('bish',5,4,190,950,'JGB0001-41055.8012257407','JGB0001-41055.8012257407_1'),('lenovo',5,4,30,150,'JGB0003-41017.7946804745','JGB0003-41017.7946804745_1');
/*!40000 ALTER TABLE `salesbilling` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `saleslist`
--

DROP TABLE IF EXISTS `saleslist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `saleslist` (
  `invoiceNo` varchar(45) NOT NULL,
  `customerId` varchar(45) NOT NULL,
  `dateSales` datetime NOT NULL,
  `totalAmount` double NOT NULL,
  `payment` double NOT NULL,
  `customerName` varchar(45) NOT NULL,
  PRIMARY KEY (`invoiceNo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `saleslist`
--

LOCK TABLES `saleslist` WRITE;
/*!40000 ALTER TABLE `saleslist` DISABLE KEYS */;
INSERT INTO `saleslist` VALUES ('JGB-IN-SALES-0001-40831.5501628009','40831.48615660880','2011-10-15 13:12:31',4400,0,'pulak'),('JGB0001-40838.6296847107','40831.48671303241','2011-06-16 00:00:00',3200,60,'prasun'),('JGB0001-40839.6169587153','40831.48671303241','2011-10-23 14:48:52',4,1000,'prasun'),('JGB0001-40839.6235526736','40831.48615660880','2011-10-23 14:58:46',1,1001.135,'pulak'),('JGB0001-40839.9170400463','40831.48615660880','2011-10-23 22:00:53',950,800,'pulak'),('JGB0001-41017.791994213','40831.48671303241','2012-04-18 19:02:42',2300,0,'prasun'),('JGB0001-41055.8012257407','40831.48671303241','2012-05-26 19:15:29',950,1200,'prasun'),('JGB0003-41017.7946804745','40831.48671303241','2012-04-18 19:04:39',150,0,'prasun');
/*!40000 ALTER TABLE `saleslist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sptinfo`
--

DROP TABLE IF EXISTS `sptinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sptinfo` (
  `id_sptinfo` int(64) NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  `address` varchar(245) DEFAULT NULL,
  `bill_disclaimer` varchar(245) DEFAULT NULL,
  `invoice_prefix` varchar(45) DEFAULT NULL,
  `password` varchar(45) DEFAULT NULL,
  `phone` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id_sptinfo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sptinfo`
--

LOCK TABLES `sptinfo` WRITE;
/*!40000 ALTER TABLE `sptinfo` DISABLE KEYS */;
INSERT INTO `sptinfo` VALUES (1,'Sun Heaven',' Kulberia, Bamangachi; North 24 pgs, WB -743706','This invoice shows the actual price of the goods  decribed in are true and correct','JGB','12345','9831392394');
/*!40000 ALTER TABLE `sptinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock`
--

DROP TABLE IF EXISTS `stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stock` (
  `id` varchar(45) NOT NULL,
  `product_name` varchar(45) NOT NULL,
  `vendor_id` varchar(70) NOT NULL,
  `date_purchased` datetime NOT NULL,
  `quantity_purchased` double NOT NULL,
  `rate` double NOT NULL,
  `quantity_available` double NOT NULL,
  `vat_rate` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock`
--

LOCK TABLES `stock` WRITE;
/*!40000 ALTER TABLE `stock` DISABLE KEYS */;
INSERT INTO `stock` VALUES ('Prod-40839.8949648264','bish','Vendor-1-40831.5295948727','2011-10-23 00:00:00',10,190,0,4),('Prod-40970.3707848495','hello',' ','2012-03-02 08:53:55',0,0,-10,4),('Prod-41017.7937903472','lenovo','Vendor-1-40831.4780250347','2012-04-18 00:00:00',10,30,5,4);
/*!40000 ALTER TABLE `stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `to_do`
--

DROP TABLE IF EXISTS `to_do`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `to_do` (
  `date_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `to_do` longtext NOT NULL,
  `id` varchar(45) NOT NULL DEFAULT 'pending',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `to_do`
--

LOCK TABLES `to_do` WRITE;
/*!40000 ALTER TABLE `to_do` DISABLE KEYS */;
INSERT INTO `to_do` VALUES ('2011-10-22 12:33:19','todo one','T-40838.7523135069'),('2011-10-22 12:41:49','todo two','T-40838.7582118403'),('2011-10-22 12:41:54','todo three','T-40838.7582691319'),('2011-10-23 16:56:29','aaaaaatttttt','T-40839.9350663194');
/*!40000 ALTER TABLE `to_do` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vendor_payment`
--

DROP TABLE IF EXISTS `vendor_payment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendor_payment` (
  `vendor_id` varchar(45) NOT NULL,
  `payment_amount` double NOT NULL,
  `payment_date` datetime NOT NULL,
  `payment_id` varchar(45) NOT NULL,
  PRIMARY KEY (`payment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendor_payment`
--

LOCK TABLES `vendor_payment` WRITE;
/*!40000 ALTER TABLE `vendor_payment` DISABLE KEYS */;
INSERT INTO `vendor_payment` VALUES ('Vendor-1-40831.4780250347',4560,'2011-10-15 11:30:58','payment-40831.4798478125'),('Vendor-0-40831.4777943403',880,'2011-10-15 11:34:42','payment-40831.4824316204'),('Vendor-0-40831.4777943403',0,'2011-10-15 12:34:21','payment-40831.5238595949'),('Vendor-1-40831.4780250347',11,'2011-10-15 12:58:40','payment-40831.5407475116'),('Vendor-2-40831.4782351852',4,'2011-10-15 12:59:17','payment-40831.5411709259'),('Vendor-2-40831.4782351852',0,'2011-10-15 13:23:03','payment-40831.5576830324'),('Vendor-1-40831.5295948727',900,'2011-10-22 11:29:40','payment-40838.4789413194'),('Vendor-1-40831.5295948727',1000,'2011-10-23 20:18:41','payment-40839.8463105556'),('Vendor-1-40831.5295948727',500,'2011-10-23 20:30:20','payment-40839.8544011111'),('Vendor-1-40831.5295948727',510,'2011-10-23 20:31:00','payment-40839.854863206'),('Vendor-1-40831.5295948727',10400,'2011-10-23 20:35:18','payment-40839.8578533102'),('Vendor-2-40831.4782351852',20800,'2011-10-23 20:36:09','payment-40839.858448588'),('Vendor-1-40831.5295948727',1900,'2011-10-23 21:34:16','payment-40839.8988041088'),('Vendor-1-40831.4780250347',0,'2012-04-18 19:03:36','payment-41017.7941690741'),('Vendor-1-40831.5295948727',1235,'2012-05-26 19:40:36','payment-41055.8198634606');
/*!40000 ALTER TABLE `vendor_payment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vendors`
--

DROP TABLE IF EXISTS `vendors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendors` (
  `vendor_id` varchar(45) NOT NULL,
  `vendor_name` varchar(70) NOT NULL,
  `vendor_address` varchar(70) DEFAULT NULL,
  `ph_no` varchar(45) DEFAULT NULL,
  `vat_no` varchar(45) DEFAULT NULL,
  `turn_over` double DEFAULT NULL,
  `due` double DEFAULT NULL,
  PRIMARY KEY (`vendor_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendors`
--

LOCK TABLES `vendors` WRITE;
/*!40000 ALTER TABLE `vendors` DISABLE KEYS */;
INSERT INTO `vendors` VALUES ('Vendor-1-40831.4780250347','anirban','bmg','789987679','789hjui',2795,-1765),('Vendor-1-40831.5295948727','SUMAN NANDI','BAMUNGACHI','','VHH',17390,945),('Vendor-2-40831.4782351852','indra','madhymgram','7891882929','tyu678',21632,832);
/*!40000 ALTER TABLE `vendors` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2012-05-26 19:52:54

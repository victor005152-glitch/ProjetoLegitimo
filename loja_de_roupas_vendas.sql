-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: loja_de_roupas
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `vendas`
--

DROP TABLE IF EXISTS `vendas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vendas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `data_venda` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `usuario_id` int DEFAULT NULL,
  `operador_nome` varchar(100) DEFAULT NULL,
  `cliente_nome` varchar(120) DEFAULT NULL,
  `cliente_cpf` varchar(20) DEFAULT NULL,
  `subtotal` decimal(10,2) NOT NULL DEFAULT '0.00',
  `desconto_percentual` decimal(10,2) NOT NULL DEFAULT '0.00',
  `desconto_valor` decimal(10,2) NOT NULL DEFAULT '0.00',
  `total` decimal(10,2) NOT NULL DEFAULT '0.00',
  `forma_pagamento` varchar(30) NOT NULL,
  `valor_recebido` decimal(10,2) DEFAULT NULL,
  `troco` decimal(10,2) DEFAULT NULL,
  `status_venda` varchar(30) NOT NULL DEFAULT 'Finalizada',
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `vendas_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendas`
--

LOCK TABLES `vendas` WRITE;
/*!40000 ALTER TABLE `vendas` DISABLE KEYS */;
INSERT INTO `vendas` VALUES (2,'2026-06-30 21:28:01',NULL,'Operador',NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(3,'2026-06-30 21:28:34',NULL,'Operador',NULL,NULL,45.00,0.00,0.00,45.00,'Pix',NULL,NULL,'Finalizada'),(4,'2026-06-30 21:34:27',NULL,'Operador',NULL,NULL,75.00,10.00,7.50,67.50,'Dinheiro',100.00,32.50,'Finalizada'),(5,'2026-06-30 21:38:34',NULL,'Operador',NULL,NULL,15.00,12.00,1.80,13.20,'Dinheiro',50.00,36.80,'Finalizada'),(6,'2026-06-30 21:41:39',NULL,'Operador',NULL,NULL,45.00,10.00,4.50,40.50,'Dinheiro',100.00,59.50,'Finalizada'),(7,'2026-06-30 21:42:56',NULL,'Operador',NULL,NULL,165.00,0.00,0.00,165.00,'Dinheiro',250.00,85.00,'Finalizada'),(8,'2026-06-30 21:44:38',NULL,'Operador',NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(9,'2026-06-30 21:48:07',NULL,'Operador',NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada');
/*!40000 ALTER TABLE `vendas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-07-01 20:19:17

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
  `cliente_id` int DEFAULT NULL,
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
  KEY `fk_vendas_clientes` (`cliente_id`),
  CONSTRAINT `fk_vendas_clientes` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`),
  CONSTRAINT `vendas_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendas`
--

LOCK TABLES `vendas` WRITE;
/*!40000 ALTER TABLE `vendas` DISABLE KEYS */;
INSERT INTO `vendas` VALUES (2,'2026-06-30 21:28:01',NULL,'Operador',NULL,NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(3,'2026-06-30 21:28:34',NULL,'Operador',NULL,NULL,NULL,45.00,0.00,0.00,45.00,'Pix',NULL,NULL,'Finalizada'),(4,'2026-06-30 21:34:27',NULL,'Operador',NULL,NULL,NULL,75.00,10.00,7.50,67.50,'Dinheiro',100.00,32.50,'Finalizada'),(5,'2026-06-30 21:38:34',NULL,'Operador',NULL,NULL,NULL,15.00,12.00,1.80,13.20,'Dinheiro',50.00,36.80,'Finalizada'),(6,'2026-06-30 21:41:39',NULL,'Operador',NULL,NULL,NULL,45.00,10.00,4.50,40.50,'Dinheiro',100.00,59.50,'Finalizada'),(7,'2026-06-30 21:42:56',NULL,'Operador',NULL,NULL,NULL,165.00,0.00,0.00,165.00,'Dinheiro',250.00,85.00,'Finalizada'),(8,'2026-06-30 21:44:38',NULL,'Operador',NULL,NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(9,'2026-06-30 21:48:07',NULL,'Operador',NULL,NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(10,'2026-07-01 20:25:17',NULL,'Operador',NULL,NULL,NULL,30.00,10.00,3.00,27.00,'Dinheiro',45.00,18.00,'Finalizada'),(11,'2026-07-01 20:47:14',NULL,'Operador',NULL,NULL,NULL,15.00,5.00,0.75,14.25,'Pix',NULL,NULL,'Finalizada'),(12,'2026-07-01 21:13:05',NULL,'Operador',NULL,NULL,NULL,300.00,0.00,0.00,300.00,'Pix',NULL,NULL,'Finalizada'),(13,'2026-07-01 21:23:03',NULL,'Operador',NULL,NULL,NULL,240.00,0.00,0.00,240.00,'Pix',NULL,NULL,'Finalizada'),(14,'2026-07-01 21:41:08',NULL,'1',NULL,NULL,NULL,12.00,0.00,0.00,12.00,'Pix',NULL,NULL,'Finalizada'),(15,'2026-07-01 21:41:46',NULL,'Franchini',NULL,NULL,NULL,12.00,0.00,0.00,12.00,'Pix',NULL,NULL,'Finalizada'),(16,'2026-07-02 20:15:29',NULL,'Franchini',NULL,NULL,NULL,40.00,0.00,0.00,40.00,'Pix',NULL,NULL,'Finalizada'),(17,'2026-07-02 20:17:17',NULL,'Franchini',NULL,NULL,NULL,40.00,0.00,0.00,40.00,'Pix',NULL,NULL,'Finalizada'),(18,'2026-07-02 21:23:07',NULL,'Franchini',NULL,NULL,NULL,52.00,0.00,0.00,52.00,'Dinheiro',100.00,48.00,'Finalizada'),(19,'2026-07-02 21:42:15',NULL,'Franchini',1,'Luciano','1240817350',40.00,0.00,0.00,40.00,'Dinheiro',100.00,60.00,'Finalizada'),(20,'2026-07-16 20:30:37',NULL,'2',NULL,NULL,NULL,26.00,0.00,0.00,26.00,'Pix',NULL,NULL,'Finalizada'),(21,'2026-07-16 20:33:29',NULL,'2',NULL,NULL,NULL,15.00,0.00,0.00,15.00,'Pix',NULL,NULL,'Finalizada'),(22,'2026-07-27 19:56:51',NULL,'GUI',NULL,NULL,NULL,48.00,10.00,4.80,43.20,'Pix',NULL,NULL,'Finalizada'),(23,'2026-07-27 19:59:24',NULL,'GUI',1,'Luciano','1240817350',16.00,0.00,0.00,16.00,'Pix',NULL,NULL,'Finalizada'),(24,'2026-08-10 19:45:28',NULL,'victor',1,'Luciano','1240817350',75.00,5.00,3.75,71.25,'Pix',NULL,NULL,'Finalizada'),(25,'2026-08-10 19:53:12',NULL,'victor',3,'Lucas','1234566778',105.00,10.00,10.50,94.50,'Dinheiro',100.00,5.50,'Finalizada');
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

-- Dump completed on 2026-08-21 19:19:50

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
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(100) NOT NULL,
  `Senha` varchar(255) NOT NULL,
  `Email` text,
  `codigo_recuperacao` varchar(6) DEFAULT NULL,
  `codigo_expira` datetime DEFAULT NULL,
  `tipo_usuario` varchar(30) NOT NULL DEFAULT 'Operador',
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  `data_cadastro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nome` (`Nome`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (4,'Franchini','1234','victor005152@gmail.com',NULL,NULL,'Operador',1,'2026-06-30 20:14:31'),(5,'Luciano','123456789','lucianovmuhlen@gmail.com',NULL,NULL,'Operador',1,'2026-06-30 20:14:31'),(6,'1','1','1',NULL,NULL,'Operador',1,'2026-06-30 20:14:31'),(7,'rai','r@iedu77','rai316004@gmail.com',NULL,NULL,'Operador',1,'2026-06-30 20:14:31'),(8,'lucianovm','123456789','lucianovmuhlen@gmail.com',NULL,NULL,'Operador',1,'2026-06-30 20:14:31'),(10,'2','$2a$11$diXm/wsNtFQQLYNVjFdSp..2rmcsSbcPgzw41rit1NPU9kattX30q','2',NULL,NULL,'Operador',1,'2026-07-14 21:18:51'),(11,'GUI','$2a$11$TUcnxu6X8sDL6YxqpNGGieKIeZXQMPQpK40FEiSUZiCimaBNN8noq','guilhermebeffart989@gmail.com','475392','2026-07-27 20:01:55','Operador',1,'2026-07-27 19:51:17'),(12,'Nicolas','$2a$11$t6kdvE9okdQOjdQOQt5sRuauG2AyoXJlKbuPBR5UuPLxiPclcRIje','tnicoolas698@gmail.com','454902','2026-08-10 19:48:50','Operador',1,'2026-08-10 19:37:54'),(13,'victor','$2a$11$FNrpsc4aufvDrkQr5sSzde12D7rQEWMbf3KxnlB.8iiBFBiXIoDDy','victormatheus3535@gmail.com',NULL,NULL,'Operador',1,'2026-08-10 19:42:15');
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-08-21 19:19:51

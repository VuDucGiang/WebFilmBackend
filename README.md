CREATE TABLE webfilm.user (
  UserID char(36) NOT NULL DEFAULT '',
  UserName varchar(100) DEFAULT NULL,
  Password char(60) DEFAULT NULL,
  Email varchar(50) DEFAULT NULL,
  CreatedDate datetime DEFAULT NULL,
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate datetime DEFAULT NULL,
  ModifiedBy varchar(255) DEFAULT NULL,
  IsAdmin tinyint DEFAULT NULL,
  Status int DEFAULT NULL
)
ENGINE = INNODB,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

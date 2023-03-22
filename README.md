CREATE TABLE webfilm.user (
  UserID char(36) NOT NULL DEFAULT '',
  UserName varchar(100) DEFAULT NULL,
  FullName varchar(255) DEFAULT NULL,
  Password char(60) DEFAULT NULL,
  Email varchar(50) DEFAULT NULL,
  DateOfBirth datetime DEFAULT NULL,
  Status int DEFAULT NULL,
  RoleType tinyint DEFAULT NULL,
  FavouriteFilmList json DEFAULT NULL,
  PasswordResetToken varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  ResetTokenExpires datetime DEFAULT NULL,
  ModifiedDate datetime DEFAULT NULL,
  ModifiedBy varchar(255) DEFAULT NULL,
  PRIMARY KEY (UserID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

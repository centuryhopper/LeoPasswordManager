
CREATE TABLE IF NOT EXISTS PasswordManager_Users (            
    ID SERIAL PRIMARY KEY,                                    
    ums_userid VARCHAR(450) NOT NULL,                         
    EMAIL VARCHAR(256) NOT NULL,                              
    FirstName VARCHAR(256) NOT NULL,                          
    LastName VARCHAR(256) NOT NULL,                           
    DateLastLogin TIMESTAMP,                                  
    DateLastLogout TIMESTAMP,                                 
    DateCreated TIMESTAMP,                                    
    DateRetired TIMESTAMP                                     
);                                                            
                                                              
CREATE TABLE IF NOT EXISTS PasswordManager_Accounts (         
    ID SERIAL PRIMARY KEY,                                    
    USERID INT NOT NULL,                                      
    TITLE VARCHAR(256) NOT NULL,                              
    USERNAME VARCHAR(256) NOT NULL,                           
    PASSWORD VARCHAR(256) NOT NULL,                           
    CREATED_AT TIMESTAMP,                                     
    LAST_UPDATED_AT TIMESTAMP,                                
    FOREIGN KEY (USERID) REFERENCES PasswordManager_Users(ID) 
);                                                            
                                                              
                                                              
CREATE TABLE IF NOT EXISTS LOGS (                             
    log_id SERIAL PRIMARY KEY,                                
    date_logged DATE NOT NULL,                                
    level VARCHAR(15) NOT NULL,                               
    message VARCHAR(256) NOT NULL                             
);                                                            
                                                              
                                                              

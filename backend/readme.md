Database Creation/requirements:
port: 8889

CREATE DATABASE argos_dev;
CREATE TABLE media (id serial primary key, url varchar(500));
INSERT INTO MEDIA (url) VALUES ("https://s3-us-west-2.amazonaws.com/eyecueargo/giveYouUp.mp4")

To run backend:
Start MySQL server in MAMP
in this directory run:
  $ npm install
  $ DEBUG=myapp:* npm start

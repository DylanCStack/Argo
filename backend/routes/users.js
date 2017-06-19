var express = require('express');
var router = express.Router();
var userController = require('../controllers/users');

/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('user root');
});
//register
router.post('/register', function(req, res) {
  // return res.json("return res.json() is working");
  // res.send('You are trying to access media id: ' + req.params.id);
  // console.log(req);

  return userController.register(req.body.phone, req.body.email, req.body.password, function(err, status){

    if(err) return res.json({'register':false, 'error': err});

    var cookie = null;

    console.log(err, status);
    if(status){//successful registration
      //properly set cookie here
      return res.json({'register':true, 'cookie': cookie, 'error': false});
    } else {// unsuccessful registration
      return res.json({'register':false, 'cookie': cookie, 'error': false});
    }
    //set response cookie
  });
});
router.post('/login', function(req, res) {
  return userController.login(req.body.phone, req.body.email, req.body.password, function(err, user){
    if(err) return res.json({"login":false, "error": err});//error logging in


    if(user){
      console.log("COOKIE SET");
      req.user = user;
      delete req.user.password; // delete the password from the session
      req.session.user = user;  //refresh the session value
      res.locals.user = user;

      return res.json({"login": true, "error": false})//successful login
    } else {
      return res.json({"login": false, "error":false})//unsuccessful login
    }
  })
});
router.get('/login/:phone/:email/:pass', function(req, res) {
  return userController.login(req.params.phone, req.params.email, req.params.pass, function(err, user){
    if(err) return res.json({"login":false, "error": err});//error logging in


    if(user){
      console.log("COOKIE SET");
      req.user = user;
      delete req.user.password; // delete the password from the session
      req.session.user = user;  //refresh the session value
      res.locals.user = user;

      return res.json({"login": true, "error": false})//successful login
    } else {
      return res.json({"login": false, "error":false})//unsuccessful login
    }
  })
});

//login

//logout



module.exports = router;

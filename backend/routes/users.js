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
  return userController.register(req.body.phone, req.body.password, function(err, status){

    if(err) return res.json({'register':false, 'error': true});

    var cookie = null;

    if(status){//successful registration
      //properly set cookie here
      return res.json({'register':true, 'cookie': cookie, 'error': false});
    } else {// unsuccessful registration
      return res.json({'register':false, 'cookie': cookie, 'error': false});
    }
    //set response cookie
  });
});

//login

//logout



module.exports = router;

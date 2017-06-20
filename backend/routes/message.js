var express = require('express');
var router = express.Router();
var messageController = require('../controllers/message');
/* GET users listing. */

var requireLogin = function(req, res, next){
  if(!req.user){
    //require login
    return res.json({"require login": true});
  } else {
    next();
  }
}

router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.get('/:id', function(req, res) {
  // res.send('You are trying to access message id: ' + req.params.id);
  messageController.getURL(req.params.id, function(err, rows){
    res.send(rows[0]['url']);
  });
});

router.post('/upload', function(req, res) {
  // res.send('You are trying to access message id: ' + req.params.id);
  console.log(req.body);
  messageController.upload(req.body.url, req.body.privacy, req.body.recipient, function(err, rows){
    if(err) return res.send(err);
    res.send(rows[0]['url']);
  });
});


module.exports = router;

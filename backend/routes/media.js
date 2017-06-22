var express = require('express');
var router = express.Router();
var mediaController = require('../controllers/media');
/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.get('/:id', function(req, res) {
  // res.send('You are trying to access media id: ' + req.params.id);
  mediaController.getURL(req.params.id, function(err, rows){
    res.send(rows[0]['url']);
  });
});

router.post('/upload', function(req, res) {
  // res.send('You are trying to access media id: ' + req.params.id);
  console.log(req.body);
  mediaController.upload(req.body.url, req.body.privacy, req.body.recipient, function(err, rows){
    if(err) return res.send(err);
    res.json(rows);
  });
});


module.exports = router;

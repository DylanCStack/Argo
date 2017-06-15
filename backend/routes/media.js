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

module.exports = router;

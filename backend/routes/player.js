const express = require('express');
const player = express.Router();
const User = require('../models/User');

// Get Player data
player.get('/data', (req, res) => {
    if (req.user) {
        // res.append("Content-Type", "application/json");
        User.find({ '_id': req.user.id })
            .then(player => {
                res.status(200).send({ message: `Succeed to get player's ${player[0]._id} data.`, data: player});
            });
    } else {
        res.status(400).send({ message: "You must be logged-in for getting player data."});
    }
})

module.exports = player;
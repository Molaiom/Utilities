<?php
$connection = mysqli_connect("localhost", "root", "root", "course");
if (mysqli_connect_errno()) {
    exit();
}

//wwwform from Unity
$appKey = $_POST["appKey"];
if ($appKey != "~@3Mc0dZkú!2") {
    exit();
}
$username = filter_var($_POST["username"], FILTER_SANITIZE_EMAIL);
$scoreToAdd = $_POST["scoreToAdd"];

// GET CURRENT SCORE FROM SERVER
$getScoreQuery = $connection->prepare("SELECT * FROM players WHERE username = ?");
$getScoreQuery->bind_param("s", $username);
if ($getScoreQuery->execute() == false){
    exit();
}
$scoreQueryResult = $getScoreQuery->get_result();

$row = $scoreQueryResult->fetch_assoc();
$currentId = $row["sqlid"];
$updatedScore = $row["score"] + $scoreToAdd;

// ADD SCORE TO SERVER
$postScoreQuery = $connection->prepare("UPDATE players SET score = ? WHERE sqlid = ?");
$postScoreQuery->bind_param("ii", $updatedScore, $currentId);
if ($postScoreQuery->execute() == true){
    echo($updatedScore);
}else{
    echo("Fudeu!");
}

$connection->close();
?>
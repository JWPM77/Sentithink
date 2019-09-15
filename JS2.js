var gridLocation = [
  [0, 0],
  [0, 0]
];
var grid1 = [
  [1, 2, -10, -14, 20],
  [0, 2, 4, 8, 9],
  [4, 2, 16, -1, -3],
  [3, 6, 10, 11, -3],
  [0, -5, -15, 3, 13]
];

for(var x=0; x<4; x++)
{
  const Http = new XMLHttpRequest();
  const urlList = ['https://sentithinkfunction.azurewebsites.net/api/GetLocationWordFrequiencies?code=Y4U0ud8b3FeN//rh1uMbguWe2c47qvYRTy6dfGeahC6uKYf3UFvbgQ==&X=0&Y=0',
                   'https://sentithinkfunction.azurewebsites.net/api/GetLocationWordFrequiencies?code=Y4U0ud8b3FeN//rh1uMbguWe2c47qvYRTy6dfGeahC6uKYf3UFvbgQ==&X=0&Y=1',
                   'https://sentithinkfunction.azurewebsites.net/api/GetLocationWordFrequiencies?code=Y4U0ud8b3FeN//rh1uMbguWe2c47qvYRTy6dfGeahC6uKYf3UFvbgQ==&X=1&Y=0',
                   'https://sentithinkfunction.azurewebsites.net/api/GetLocationWordFrequiencies?code=Y4U0ud8b3FeN//rh1uMbguWe2c47qvYRTy6dfGeahC6uKYf3UFvbgQ==&X=1&Y=1']

  Http.open("GET", urlList[x]);
  Http.send();

  Http.onreadystatechange = function()
  {
    if(this.readyState == 4 && this.status == 200)
    {
      //console.log(Http.responseText);
      var data = JSON.parse(Http.responseText);
      //console.log(data);

      //console.log(data.length)
      console.log(data)
      for(var row in data)
      {
        var xCoord = data[row]["X"];
        //console.log(data[row]);
        var yCoord = data[row]["Y"];
        var frequency = data[row]["FREQUENCY"];
        //console.log("Frequency is " + frequency);
        var sentiment = data[row]["SENTIMENT"];
        var totalSentiment = frequency;
        //console.log("Sentiment is " + totalSentiment);
        gridLocation[xCoord][yCoord] += totalSentiment;
        //console.log(gridLocation[xCoord][yCoord]);
        createGrid(gridLocation);
      }
    }
  }
}

var gridData = createGrid(gridLocation);

var grid = d3.select("#d3")
  .append("svg")
  .attr("width", "1000px")
  .attr("height", "1000px");

var row = grid.selectAll(".row")
  .data(gridData)
  .enter().append("g")
  .attr("class", "row")

var column = row.selectAll(".square")
  .data(function(d) {return d;})
  .enter().append("rect")
  .attr("class", "square")
  .attr("x", function(d){return d.x; })
  .attr("y", function(d){return d.y; })
  .attr("width", function(d){return d.width; })
  .attr("height", function(d){return d.height; })
  .attr("id", function(d){return d.id; })
  .style("fill", "white")
  .style("stroke", "black")

//createGrid(grid1)

function createGrid(sentimentData)
{
  //essential variables
  var startingX = 400;
  var currentX = 400;
  var currentY = 100
  var width = 100;
  var height = 100;
  var sentimentArray = sentimentData;
  var map = new Array();

  //iterates through the rows in the grid
  for(var row = 0; row < sentimentData.length; row++)
  {
    //pushes a new array into each element of the first array to create a 2d array
    map.push(new Array());

    for(column = 0; column < sentimentData[0].length; column++)
    {
      //has the row and column numbers in string format to use with id calls
      var rowNum = row.toString()
      var colNum = column.toString()

      var infoArray = {
        id: row +","+ column,
        x: currentX,
        y: currentY,
        width: width,
        height: height,
        sentiment: sentimentArray[row][column]
      }
      //pushes the array with information to each row of the map
      map[row].push(infoArray);

      var greenNess = map[row][column]["sentiment"];

      if((map[row][column]["sentiment"] > 0) && (map[row][column]["sentiment"] < 150))
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", "#9df5c2");
      }
      if((map[row][column]["sentiment"] > 150) && (map[row][column]["sentiment"] < 300))
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", "#45f790");
      }
      if(map[row][column]["sentiment"] > 300)
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", "#00ff48");
      }
      if((map[row][column]["sentiment"] < 0) && (map[row][column]["sentiment"] > -10))
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", '#ed5353');
      }
      if(map[row][column]["sentiment"] < -10)
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", '#ed0000');
      }
      if(map[row][column]["sentiment"] == 0)
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", 'white');
      }
      //move coordinates over to the right by width
      currentX += width;
    }
    //resets the grid horizontally from right to left
    currentX = startingX;

    //changes the y axis of the grid by moving it down
    currentY += height;
  }
  return map;
}
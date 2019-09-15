var gridLocation = [
  [0, 0],
  [0, 0]
];

//for(var x=0)
const Http = new XMLHttpRequest();
const url='https://sentithinkfunction.azurewebsites.net/api/GetLocationWordFrequiencies?code=Y4U0ud8b3FeN//rh1uMbguWe2c47qvYRTy6dfGeahC6uKYf3UFvbgQ==&X=1&Y=1';
Http.open("GET", url);
Http.send();

Http.onreadystatechange = function()
{
  if(this.readyState == 4 && this.status == 200)
  {
    //console.log(Http.responseText);
    var data = JSON.parse(Http.responseText);
    console.log(data);

    console.log(data.length)

    for(var row in data)
    {
      var xCoord = data[row]["X"];
      var yCoord = data[row]["Y"];
      var frequency = data[row]["FREQUENCY"];
      var sentiment = data[row]["SENTIMENT"];
      var totalSentiment = frequency * sentiment;
      console.log("Sentiment is " + totalSentiment);
    }
    gridLocation[xCoord][yCoord] = totalSentiment;
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

var textGrid = d3.select("#text")
    .append("svg")
    .attr("width", "1000px")
    .attr("height", "1000px");

var textRow = textGrid.selectAll(".row")
    .data(gridData)
    .enter().append("g")
    .attr("class", "r")

var textColumn = textRow.selectAll(".text")
    .data(function(d) {return d;})
    .enter().append("text")
    .attr("class", "text")
    .attr("x", function(d){return d.x; })
    .attr("y", function(d){return d.y; })
    .attr("id", function(d){return d.id; })
    .text("Hello")
    .style("fill", "red")


createGrid(gridLocation);

//console.log("The id for 3,3 is " + createGrid(sData)[2][2]["id"])
/*var column2 = row.selectAll(".square")
  .data(function(d) {return d;})
  .enter().append("text")
  .attr("class", "text")
  .attr("x", function(d){return d.x; })
  .attr("y", function(d){return d.y; })
  .attr("font-size", "50px")
  .attr("id", function(d){return d.id; })
  .text("10")
  .style("fill", "red")*/

function createGrid(sentimentData)
{
  //console.log("The length of the row is " + sentimentData.length)
  //console.log("The length of the column is " + sentimentData[0].length)

  //essential variables
  var startingX = 200;
  var currentX = 200;
  var currentY = 200
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

      if((map[row][column]["sentiment"] > 0) && (map[row][column]["sentiment"] < 10))
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", '#00ff7b');
      }
      if(map[row][column]["sentiment"] > 10)
      {
        d3.select("[id=" + "'" +rowNum+ ',' +colNum+ "'" + "]").style("fill", '#25b86c');
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
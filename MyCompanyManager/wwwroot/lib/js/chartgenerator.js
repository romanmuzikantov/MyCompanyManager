function generateChart(id, name, type, labels, data)
{
    var ctxL = document.getElementById(id).getContext('2d');

    var myLineChart = new Chart(ctxL, {
        type: type,
        data: {
            labels: labels,
            datasets: [
                {
                    label: name,
                    data: data,
                    backgroundColor: getColors(data),
                    borderColor: [
                        '#FFF',
                    ],
                    borderWidth: 2,
                    pointBorderColor: "#fff",
                    pointBackgroundColor: "rgba(173, 53, 186, 0.1)",
                }
            ]
        },
        options: {
            responsive: true
        }
    });

    return myLineChart;
}

function getColors(size) {
    var colors = ["#cdffeb", "#009f9d", "#07456f", "#0f0a3c", "#ff8484", "#d84c73", "#5c3b6f", "#f9ed69", "#f08a5d", "#f38181", "#e23e57", "#88304e", "#522546", "#311d3f", "#6a2c70", "#00b8a9", "#ff9999", "#444f5a", "#61c0bf", "#521262", "#1fab89", "#62d2a2", "#9df3c4", "#fc5c9c", "#fccde2", "#d4a5a5", "#455d7a", "#4d4545", "#8d6262"];
    var chosenColors = [];
    var indexLimit = 28;
    size.forEach(function() {
        var index = getRandomInt(indexLimit);
        chosenColors.push(colors[index]);
        colors.splice(index, 1);
        indexLimit -= 1;
    });

    return chosenColors;
}

function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}
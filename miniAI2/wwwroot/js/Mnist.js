function LoadTrainData() {
    const trainDataPath = document.getElementById('trainDataPath').value;

    fetch('http://localhost:5235/mnist/LoadTrainData', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(trainDataPath)
    })
        .then(response => {
            return response.json();
        })
        .then(result => {
            document.getElementById('trainResult').innerText = result.status;
            loadTrainingDataIntoListView(result.dataList);

        })
        .catch(error => {
            document.getElementById('trainResult').innerText = error;
        });
}
function trainModel() {
    const trainDataPath = document.getElementById('trainDataPath').value;

    fetch('http://localhost:5235/mnist/train', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(trainDataPath)
    })
        .then(response => response.text())
        .then(result => {
            document.getElementById('trainResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('trainResult').innerText = error;
        });
}
function predict() {
    const pixelDataText = document.getElementById('pixelData').value;
    const pixelData = pixelDataText.split(',').map(Number);

    fetch('http://localhost:5235/mnist/predict', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pixelData)
    })
        .then(response => response.json())
        .then(result => {
            document.getElementById('predictionResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('predictionResult').innerText = error;
        });
}

function calculateLoss(predictedValues) {


    // Ask the user for the actual label (as a list for future scalability)
    const actualLabel = parseFloat(prompt("Enter the actual label:"));

    // Here we assume predictedValues is a list and calculate the loss for each predicted value
    let losses = predictedValues.map(predictedValue => Math.pow(predictedValue - actualLabel, 2));

    // Display the calculated losses
    alert("Calculated Losses (MSE): " + losses.join(", "));

    return losses; // Returning the list of losses
}

function forwardPass() {
    const predictionResult = document.getElementById('predictionResult').innerText;

    // Ensure predictionResult is an array or convert it to an array with one value
    if (!Array.isArray(predictionResult)) {
        predictedValuesGlobal = [parseFloat(predictionResult)];
    } else {
        predictedValuesGlobal = predictionResult.map(parseFloat);
    }

    // Calculate and store the loss in the global variable
    if (predictionResult != "") {
        lossGlobal = calculateLoss(predictedValuesGlobal);
    }
}

function calculateGradients() {
    if (predictedValuesGlobal.length === 0) {
        alert("No prediction or loss calculated yet.");
        return;
    }

    const pixelDataText = document.getElementById('pixelData').value;
    const pixelData = pixelDataText.split(',').map(Number);

    // Sending pixel data, predicted values, and loss to the server for gradient calculation
    fetch('http://localhost:5235/mnist/gradient', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            pixelData,
            predictedValuesGlobal,
            lossGlobal
        })
    })
        .then(response => response.json())
        .then(result => {
            document.getElementById('gradientResult').innerText = result;
        })
        .catch(error => {
            document.getElementById('gradientResult').innerText = error;
        });
}


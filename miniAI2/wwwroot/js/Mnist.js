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
            document.getElementById('trainResult').innerText = "Error: " + error;
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

function updateImage() {
    const pixelData = document.getElementById("pixelData").value.split(',').map(Number);
    const canvas = document.getElementById("pixelCanvas");
    const context = canvas.getContext("2d");
    const imageData = context.createImageData(28, 28);

    // Ensure the input data is exactly 784 pixels
    if (pixelData.length === 784) {
        for (let i = 0; i < 784; i++) {
            const pixelValue = pixelData[i];
            const index = i * 4;

            // Set RGBA values: R, G, B, A (Alpha)
            imageData.data[index] = pixelValue; // Red
            imageData.data[index + 1] = pixelValue; // Green
            imageData.data[index + 2] = pixelValue; // Blue
            imageData.data[index + 3] = 255; // Alpha (fully opaque)
        }

        // Update canvas with the new image data
        context.putImageData(imageData, 0, 0);
    } else {
        console.error("Pixel data must contain exactly 784 values.");
    }
}

function forwardPass() {
    const predictionResult = document.getElementById('predictionResult').innerText;


    if (predictionResult != "") {
        let res = parseFloat(predictionResult);
        let ret = calculateLoss(res);
    }
}
function calculateLoss(predictedValue) {
    // Get the actual label from the user (you can modify this to get the actual label from your dataset)
    const actualLabel = parseFloat(prompt("Enter the actual label:"));

    // Calculate Mean Squared Error (MSE) as an example loss function
    const loss = Math.pow(predictedValue - actualLabel, 2);

    // Display the calculated loss
    alert("Calculated Loss (MSE): " + loss);
}

function loadTrainingDataIntoListView(data) {
    const listView = document.getElementById("trainingDataListView");

    // Clear the list view
    listView.innerHTML = '';

    data.forEach(imageData => {
        // Create a canvas to draw the image
        const canvas = document.createElement("canvas");
        canvas.width = 28;
        canvas.height = 28;
        const ctx = canvas.getContext("2d");

        // Draw the image using pixel data
        for (let i = 0; i < 28; i++) {
            for (let j = 0; j < 28; j++) {
                const pixelValue = imageData.pixels[i * 28 + j];
                ctx.fillStyle = `rgb(${pixelValue}, ${pixelValue}, ${pixelValue})`;
                ctx.fillRect(j, i, 1, 1);
            }
        }

        // Convert canvas to image
        const img = document.createElement("img");
        img.src = canvas.toDataURL();
        img.alt = "Training Image"; // Optional alt text

        // Add image to the list view
        listView.appendChild(img);
    });
}







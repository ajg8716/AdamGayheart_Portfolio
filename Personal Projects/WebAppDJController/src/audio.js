// 1 - our WebAudio context, **we will export and make this public at the bottom of the file**

// **These are "private" properties - these will NOT be visible outside of this module (i.e. file)**
// 2 - WebAudio nodes that are part of our WebAudio audio routing graph
let audioCtx;

let element1, sourceNode1, analyserNode1, gainNode1;
let highPassFilter1, lowPassFilter1, lowEQ1, midEQ1, highEQ1;

let element2, sourceNode2, analyserNode2, gainNode2;
let highPassFilter2, lowPassFilter2, lowEQ2, midEQ2, highEQ2;

//master output
let masterGainNode;

// 3 - here we are faking an enumeration
const DEFAULTS = Object.freeze({
    gain : .5,
    numSamples : 256
});

// 4 - create a new array of 8-bit integers (0-255)
// this is a typed array to hold the audio frequency data
let audioData1 = new Uint8Array(DEFAULTS.numSamples/2);
let audioData2 = new Uint8Array(DEFAULTS.numSamples/2);

// **Next are "public" methods - we are going to export all of these at the bottom of this file**
function setupWebaudio(){
    const AudioContext = window.AudioContext || window.webkitAudioContext;
    audioCtx = new AudioContext();

    //initialize each audio element
    element1 = new Audio();
    element2 = new Audio();

    //setup for turntable 1 values
    sourceNode1 = audioCtx.createMediaElementSource(element1);
    analyserNode1 = audioCtx.createAnalyser();
    analyserNode1.fftSize = DEFAULTS.numSamples;
    gainNode1 = audioCtx.createGain();
    gainNode1.gain.value = DEFAULTS.gain;

    //setup for turntable 2 values
    sourceNode2 = audioCtx.createMediaElementSource(element2);
    analyserNode2 = audioCtx.createAnalyser();
    analyserNode2.fftSize = DEFAULTS.numSamples;
    gainNode2 = audioCtx.createGain();
    gainNode2.gain.value = DEFAULTS.gain;

    //master gaine node setup
    masterGainNode = audioCtx.createGain();
    masterGainNode.gain.value = 1.0;

    // EQ Filters for turntable 1
    //lows
    lowEQ1 = audioCtx.createBiquadFilter();
    lowEQ1.type = "lowshelf";
    lowEQ1.frequency.setValueAtTime(320, audioCtx.currentTime);
    //mids
    midEQ1 = audioCtx.createBiquadFilter();
    midEQ1.type = "peaking";
    midEQ1.frequency.setValueAtTime(1000, audioCtx.currentTime);
    //highs
    highEQ1 = audioCtx.createBiquadFilter();
    highEQ1.type = "highshelf";
    highEQ1.frequency.setValueAtTime(3200, audioCtx.currentTime);
    //low pass filter
    lowPassFilter1 = audioCtx.createBiquadFilter();
    lowPassFilter1.type = "lowpass";
    lowPassFilter1.frequency.value = 22050; // Initial frequency for low-pass
    //high pass filter
    highPassFilter1 = audioCtx.createBiquadFilter();
    highPassFilter1.type = "highpass";
    highPassFilter1.frequency.value = 0; // Initial frequency for high-pass

    // EQ Filters for turntable 2
    //lows
    lowEQ2 = audioCtx.createBiquadFilter();
    lowEQ2.type = "lowshelf";
    lowEQ2.frequency.setValueAtTime(320, audioCtx.currentTime);
    //mids
    midEQ2 = audioCtx.createBiquadFilter();
    midEQ2.type = "peaking";
    midEQ2.frequency.setValueAtTime(1000, audioCtx.currentTime);
    //highs
    highEQ2 = audioCtx.createBiquadFilter();
    highEQ2.type = "highshelf";
    highEQ2.frequency.setValueAtTime(3200, audioCtx.currentTime);
    //low pass filter
    lowPassFilter2 = audioCtx.createBiquadFilter();
    lowPassFilter2.type = "lowpass";
    lowPassFilter2.frequency.value = 22050; // Initial frequency for low-pass
    //high pass filter
    highPassFilter2 = audioCtx.createBiquadFilter();
    highPassFilter2.type = "highpass";
    highPassFilter2.frequency.value = 0; // Initial frequency for high-pass

    // Connect the nodes for turntable 1
    // Source > Analyser > lowEQ > midEQ > highEQ > lowPass > highPass > gainNode > masterGainNode > Destination
    sourceNode1.connect(analyserNode1);
    analyserNode1.connect(lowEQ1);
    lowEQ1.connect(midEQ1);
    midEQ1.connect(highEQ1);
    highEQ1.connect(lowPassFilter1);
    lowPassFilter1.connect(highPassFilter1);
    highPassFilter1.connect(gainNode1);
    gainNode1.connect(masterGainNode);

    // Connect the nodes for turntable 2
    // Source > Analyser > lowEQ > midEQ > highEQ > lowPass > highPass > gainNode > masterGainNode > Destination
    sourceNode2.connect(analyserNode2);
    analyserNode2.connect(lowEQ2);
    lowEQ2.connect(midEQ2);
    midEQ2.connect(highEQ2);
    highEQ2.connect(lowPassFilter2);
    lowPassFilter2.connect(highPassFilter2);
    highPassFilter2.connect(gainNode2);
    gainNode2.connect(masterGainNode);

    //connect master gain to the output
    masterGainNode.connect(audioCtx.destination);
}

//loads the file inputed and sets it to turntable inputed
function loadSoundFile(filePath, turntableNum){
    if(turntableNum === 1){
        element1.src = filePath;
    }
    if(turntableNum === 2){
        element2.src = filePath;
    }
}

// Play/Pause audio track 1
function playTrack1() {
    if (!sourceNode1){
        console.log("source node 1 not initialized");
        return;
    } 

    if(!element1.src || element1.src === ""){
        console.log("No audio source loaded for track 1");
        return;
    }
    //event listener that logs errors during playing
    element1.onerror = (e) => {
        console.error("audio playback error:", e);
    };

    try {
        // check if audio context is suspended
        if (audioCtx.state === "suspended") {
            audioCtx.resume().then(() => {
                console.log("AudioContext resumed");
                element1.play().catch(err => console.error("play error:", err));
            });
        } else {
            const playPromise = element1.play();
            if (playPromise !== undefined) {
                playPromise.catch(error => {
                    console.error("Play error:", error);
                });
            }
        }
    } catch (error) {
        console.error("error playing track 1:", error);
    }
}

function pauseTrack1() {
    if (!sourceNode1) return;
    try {
        element1.pause();
    } catch (error) {
        console.log("error pausing track 1:", error);
    }
}

function stopTrack1(){
    if(!sourceNode1) return;
    try {
        element1.pause();
        element1.currentTime = 0;
    } catch (error) {
        console.log("error stopping track 1:", error);
    }
}

// Play/Pause audio track 2
function playTrack2() {
    if (!sourceNode2){
        console.log("source node 2 not initialized");
        return;
    } 

    if(!element2.src || element2.src === ""){
        console.log("No audio source loaded for track 2");
        return;
    }
    //event listener that logs errors during playing
    element2.onerror = (e) => {
        console.error("audio playback error:", e);
        return;
    };

    try {
        // check if audio context is suspended
        if (audioCtx.state === "suspended") {
            audioCtx.resume().then(() => {
                console.log("AudioContext resumed");
                element2.play().catch(err => console.error("play error:", err));
            });
        } else {
            const playPromise = element2.play();
            if (playPromise !== undefined) {
                playPromise.catch(error => {
                    console.error("play error:", error);
                });
            }
        }
    } catch (error) {
        console.error("error playing track 2:", error);
    }
}

function pauseTrack2() {
    if (!sourceNode2) return;
    try {
        element2.pause();
    } catch (error) {
        console.log("error pausing track 2:", error);
    }
}

function stopTrack2() {
    if(!sourceNode2) return;
    try {
        element2.pause();
        element2.currentTime = 0;
    } catch (error) {
        console.log("error stopping track 2:", error);
    }
}

// Volume Control that applys to turntable inputed
function setVolume(value, turnTableNum) {
    // Normalize slider value (0-100) to a 0.0 - 1.0 range
    const normalizedValue = Number(value) / 100;

    if(turnTableNum === 1){
        gainNode1.gain.setValueAtTime(normalizedValue, audioCtx.currentTime);
    }
    else if(turnTableNum === 2){
        gainNode2.gain.setValueAtTime(normalizedValue, audioCtx.currentTime);
    }
}

// EQ Controls for turntable 1
function setLowGain1(value) {
    const gain = value * 12 - 6; // convert 0 - 1 to -6 to +6 db
    lowEQ1.gain.setValueAtTime(gain, audioCtx.currentTime);
}
function setMidGain1(value) {
    const gain = value * 12 - 6;
    midEQ1.gain.setValueAtTime(gain, audioCtx.currentTime);
}
function setHighGain1(value) {
    const gain = value * 12 - 6;
    highEQ1.gain.setValueAtTime(gain, audioCtx.currentTime);
}

// EQ Controls for turntable 2
function setLowGain2(value) {
    const gain = value * 12 - 6; // convert 0 - 1 to -6 to +6 db
    lowEQ2.gain.setValueAtTime(gain, audioCtx.currentTime);
}
function setMidGain2(value) {
    const gain = value * 12 - 6;
    midEQ2.gain.setValueAtTime(gain, audioCtx.currentTime);
}
function setHighGain2(value) {
    const gain = value * 12 - 6;
    highEQ2.gain.setValueAtTime(gain, audioCtx.currentTime);
}

// CFX Filters that applys to the turntable inputed
function setCFX(value, turnTableNum){
    //value 0-100
    const normalizedValue = value/100;

    if(turnTableNum === 1) {
        if (normalizedValue < 0.5) {
            // Low-pass filter applied for the left side of slider
            const freq = 22050 - (normalizedValue * 2 * 20000); // reduce frequency progressively
            lowPassFilter1.frequency.setValueAtTime(Math.max(freq, 50), audioCtx.currentTime);
            highPassFilter1.frequency.setValueAtTime(0, audioCtx.currentTime); //disable high pass
        } else {
            //high pass filter applied right side of slider
            const freq = (normalizedValue - 0.5) * 2 * 20000; // increase frequency progressively
            highPassFilter1.frequency.setValueAtTime(Math.min(freq, 18000), audioCtx.currentTime);
            lowPassFilter1.frequency.setValueatTime(22050, audioCtx.currentTime); // disable low pass
        }
    }
    else if(turnTableNum === 2) {
        if (normalizedValue < 0.5) {
            // Low-pass filter applied for the left side of slider
            const freq = 22050 - (normalizedValue * 2 * 20000); // reduce frequency progressively
            lowPassFilter2.frequency.setValueAtTime(Math.max(freq, 50), audioCtx.currentTime);
            highPassFilter2.frequency.setValueAtTime(0, audioCtx.currentTime); //disable high pass
        } else {
            //high pass filter applied right side of slider
            const freq = (normalizedValue - 0.5) * 2 * 20000; // increase frequency progressively
            highPassFilter2.frequency.setValueAtTime(Math.min(freq, 18000), audioCtx.currentTime);
            lowPassFilter2.frequency.setValueatTime(22050, audioCtx.currentTime); // disable low pass
        }
    }
}

// Export functions
export {
    audioCtx,
    analyserNode1,
    analyserNode2,
    setupWebaudio,
    loadSoundFile,
    playTrack1,
    pauseTrack1,
    stopTrack1,
    playTrack2,
    pauseTrack2,
    stopTrack2,
    setVolume,
    setLowGain1,
    setMidGain1,
    setHighGain1,
    setLowGain2,
    setMidGain2,
    setHighGain2,
    setCFX,
    lowPassFilter1,
    highPassFilter1,
    lowPassFilter2,
    highPassFilter2,
    element1,
    element2
};
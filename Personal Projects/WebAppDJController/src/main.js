//console.log("main.js loaded");

import * as jogwheel from './canvas-jogwheel.js';
import * as audio from './audio.js';

let jogwheel1;
let jogwheel2;
let cuePoint1 = 0;
let cuePoint2 = 0;

//init function
function init(){
    audio.setupWebaudio();
    
    const jogwheel1Canvas = document.querySelector("#jogwheel1");
    const jogwheel2Canvas = document.querySelector("#jogwheel2");
    
    // setup jogwheels with their canvases
    jogwheel1 = new jogwheel.Jogwheel(jogwheel1Canvas, audio.element1);
    jogwheel2 = new jogwheel.Jogwheel(jogwheel2Canvas, audio.element2);
    
    // setup file input handlers
    setupFileInputs();
    
    // setup UI controls for both turntables
    setupUI(jogwheel1Canvas, "1");
    setupUI(jogwheel2Canvas, "2");
    
    // disable tracks until tracks are loaded
    updateButtonState("1");
    updateButtonState("2");
    
    // event listeners to update jogwheels
    // jogwheel 1
    audio.element1.addEventListener('timeupdate', () => {
        // jogwheel automatically update its position based on currentTime
        if (!jogwheel1.isPlaying) {
            // request a draw when not playing
            requestAnimationFrame(() => jogwheel1.draw());
        }
    });
    //same for jogwheel 2
    audio.element2.addEventListener('timeupdate', () => {
        if (!jogwheel2.isPlaying) {
            requestAnimationFrame(() => jogwheel2.draw());
        }
    });
}

function setupFileInputs() {
    // setup file input for turntable 1
    document.querySelector("#input1").addEventListener("change", (e) => {
        const file = e.target.files[0];
        if (!file) return;
        
        console.log("Track 1 file selected:", file.name);
        const fileURL = URL.createObjectURL(file);
        console.log("Track 1 URL:", fileURL);
        audio.loadSoundFile(fileURL, 1);
        
        // reset play button state
        const playButton = document.querySelector("#play1");
        playButton.dataset.playing = "no";
        playButton.textContent = "Play";
        
        // reset jogwheel
        if (jogwheel1) {
            jogwheel1.stopRotation();
            jogwheel1.setAudioElement(audio.element1);
        }
        
        // reset cue point
        cuePoint1 = 0;
        
        // update button states
        updateButtonState("1");
        
        // parse ID3 tags (handled in audio-analyser.js)
    });
    
    // setup file input for turntable 2
    document.querySelector("#input2").addEventListener("change", (e) => {
        const file = e.target.files[0];
        if (!file) return;
        
        console.log("Track 2 file selected:", file.name);
        const fileURL = URL.createObjectURL(file);
        console.log("Track 2 URL:", fileURL);
        audio.loadSoundFile(fileURL, 2);
        
        // reset play button state
        const playButton = document.querySelector("#play2");
        playButton.dataset.playing = "no";
        playButton.textContent = "Play";
        
        // reset jogwheel
        if (jogwheel2) {
            jogwheel2.stopRotation();
            jogwheel2.setAudioElement(audio.element2);
        }
        
        // reset cue point
        cuePoint2 = 0;
        
        // update button states
        updateButtonState("2");
        
        // use jsmediatags to extract metadata 
        if (window.jsmediatags) {
            window.jsmediatags.read(file, {
                onSuccess: function(tag) {
                    const data = tag.tags.picture ? tag.tags.picture.data : null;
                    const format = tag.tags.picture ? tag.tags.picture.format : null;
                    
                    if (data && format) {
                        let base64String = "";
                        for(let i = 0; i < data.length; i++) {
                            base64String += String.fromCharCode(data[i]);
                        }
                        document.querySelector("#cover2").style.backgroundImage = 
                            `url(data:${format};base64,${window.btoa(base64String)})`;
                    }
                    //assigns the title from metadata to the #title2
                    if (tag.tags.title) {
                        document.querySelector("#title2").textContent = tag.tags.title;
                    }
                    //assigns the artist from metadata to the #artist2
                    if (tag.tags.artist) {
                        document.querySelector("#artist2").textContent = tag.tags.artist;
                    }
                },
                onError: function(error) {
                    console.log("Error reading tags:", error);
                }
            });
        }
    });
}

// function to update button states based on track
function updateButtonState(id) {
    const playButton = document.querySelector(`#play${id}`);
    const cueButton = document.querySelector(`#cue${id}`);
    const hasTrack = id === "1" ? 
        audio.element1 && audio.element1.src && audio.element1.src !== "" :
        audio.element2 && audio.element2.src && audio.element2.src !== "";
    
    // visually disable/enable buttons
    if (hasTrack) {
        playButton.disabled = false;
        cueButton.disabled = false;
        playButton.classList.remove('disabled-button');
        cueButton.classList.remove('disabled-button');
    } else {
        playButton.disabled = true;
        cueButton.disabled = true;
        playButton.classList.add('disabled-button');
        cueButton.classList.add('disabled-button');
    }
}

function setupUI(canvasElement, id){
  
    //create variable for all the controls for the turntable/ channel in the mixer
    let playButton;
    let cueButton;
    let volumeSlider;
    let cfxKnob;
    let lowKnob;
    let midKnob;
    let highKnob;
    let jogwheelInstance;
    let audioElement;

    //if assigning to jogwheel1
    if(id === "1"){
        playButton = document.querySelector("#play1");
        cueButton = document.querySelector("#cue1");
        volumeSlider = document.querySelector("#volume-slider1");
        cfxKnob = document.querySelector("#cfx1");
        lowKnob = document.querySelector("#channel1 .slider-container:nth-child(3) input");
        midKnob = document.querySelector("#channel1 .slider-container:nth-child(2) input");
        highKnob = document.querySelector("#channel1 .slider-container:nth-child(1) input");
        jogwheelInstance = jogwheel1;
        audioElement = audio.element1;
    }
    //if assigning to jogwheel2
    else if(id === "2"){
        playButton = document.querySelector("#play2");
        cueButton = document.querySelector("#cue2");
        volumeSlider = document.querySelector("#volume-slider2");
        cfxKnob = document.querySelector("#cfx2");
        lowKnob = document.querySelector("#channel2 .slider-container:nth-child(3) input");
        midKnob = document.querySelector("#channel2 .slider-container:nth-child(2) input");
        highKnob = document.querySelector("#channel2 .slider-container:nth-child(1) input");
        jogwheelInstance = jogwheel2;
        audioElement = audio.element2;
    }

    //add .onclick event to the play button
    playButton.onclick = e => {
        // don't do anything if button is disabled
        if (e.target.disabled) return;
        
        console.log(`audioCtx.state before = ${audio.audioCtx.state}`);
        
        // check if context is in suspended state
        if(audio.audioCtx.state == "suspended"){
            audio.audioCtx.resume();
        }
        console.log(`audioCtx.state after = ${audio.audioCtx.state}`);
        
        if(e.target.dataset.playing == "no"){
            // if track is currently paused, play it
            if(id === "1"){
                console.log("Starting playback for track 1");
                try {
                    if (!audio.element1.src) {
                        console.log("No audio source loaded for track 1");
                        alert("Please select an audio file for track 1 first");
                        return;
                    }
                    console.log("Track 1 source:", audio.element1.src);
                    audio.playTrack1();
                    jogwheel1.startRotation();
                    console.log("Track 1 playback started");
                } catch (error) {
                    console.log("Error playing track 1:", error);
                }
            } else {
                console.log("Starting playback for track 2");
                try {
                    if (!audio.element2.src) {
                        console.log("No audio source loaded for track 2");
                        alert("Please select an audio file for track 2 first");
                        return;
                    }
                    console.log("Track 2 source:", audio.element2.src);
                    audio.playTrack2();
                    jogwheel2.startRotation();
                    console.log("Track 2 playback started");
                } catch (error) {
                    console.log("Error playing track 2:", error);
                }
            }
            
            e.target.dataset.playing = "yes";
            e.target.textContent = "Pause";
        }
        // if track playing, pause it
        else{
            if(id === "1"){
                console.log("Pausing track 1");
                audio.pauseTrack1();
                jogwheel1.stopRotation();
            } else {
                console.log("Pausing track 2");
                audio.pauseTrack2();
                jogwheel2.stopRotation();
            }
            e.target.dataset.playing = "no";
            e.target.textContent = "Play";
        }
    };

    // add .onclick and .onmousedown/.onmouseup event to the cue button
    // 1. if track playing, cue stops it and returns to cue point
    // 2. if track stopped, clicking cue sets the current point as cue point
    // 3. if track stopped at cue point, holding cue plays temporarily
    
    cueButton.onmousedown = () => {
        // don't do anything if button is disabled
        if (cueButton.disabled) return;
        
        if(id === "1"){
            if (playButton.dataset.playing === "yes") {
                // if playing, stop track and return to cue point
                audio.pauseTrack1();
                audio.element1.currentTime = cuePoint1;
                playButton.dataset.playing = "no";
                playButton.textContent = "Play";
                jogwheel1.stopRotation();
            } else {
                // check if  exactly at cue point (within small epsilon for floating point comparison)
                const epsilon = 0.01;
                if (Math.abs(audio.element1.currentTime - cuePoint1) < epsilon) {
                    // at cue point - preview play while mouse is down
                    audio.playTrack1();
                    jogwheel1.startRotation();
                } else {
                    // not at cue point - set new cue point
                    cuePoint1 = audio.element1.currentTime;
                    console.log("Set new cue point for deck 1:", cuePoint1);
                }
            }
        } else {
            if (playButton.dataset.playing === "yes") {
                // if playing, stop track and return to cue point
                audio.pauseTrack2();
                audio.element2.currentTime = cuePoint2;
                playButton.dataset.playing = "no";
                playButton.textContent = "Play";
                jogwheel2.stopRotation();
            } else {
                // if exactly at cue point (within small epsilon for floating point comparison)
                const epsilon = 0.01;
                if (Math.abs(audio.element2.currentTime - cuePoint2) < epsilon) {
                    // at cue point - preview play while mouse is down
                    audio.playTrack2();
                    jogwheel2.startRotation();
                } else {
                    // not at cue point - set new cue point
                    cuePoint2 = audio.element2.currentTime;
                    console.log("Set new cue point for deck 2:", cuePoint2);
                }
            }
        }
    };
    
    cueButton.onmouseup = () => {
        // don't do anything if button is disabled
        if (cueButton.disabled) return;
        
        if(id === "1"){
            // only handle the case when previewing from cue point
            if (playButton.dataset.playing === "no") {
                audio.pauseTrack1();
                audio.element1.currentTime = cuePoint1;
                jogwheel1.stopRotation();
            }
        } else {
            // only handle the case when previewing from cue point
            if (playButton.dataset.playing === "no") {
                audio.pauseTrack2();
                audio.element2.currentTime = cuePoint2;
                jogwheel2.stopRotation();
            }
        }
    };

    // Volume Slider
    volumeSlider.addEventListener("input", e => {
        audio.setVolume(e.target.value, id === "1" ? 1 : 2);
    });
    
    // CFX Slider (High-Pass & Low-Pass Filter)
    cfxKnob.addEventListener("input", e => {
        const value = parseFloat(e.target.value);
        audio.setColorFX(value, id === "1" ? 1 : 2);
    });

    // EQ Sliders
    // low knob
    lowKnob.oninput = (e) => {
        if(id === "1"){
            audio.setLowGain1(e.target.value / 100);
        } else {
            audio.setLowGain2(e.target.value / 100);
        }
    };
    // mid knob
    midKnob.oninput = (e) => {
        if(id === "1"){
            audio.setMidGain1(e.target.value / 100);
        } else {
            audio.setMidGain2(e.target.value / 100);
        }
    };
    // high knob
    highKnob.oninput = (e) => {
        if(id === "1"){
            audio.setHighGain1(e.target.value / 100);
        } else {
            audio.setHighGain2(e.target.value / 100);
        }
    };
    
    // update jogwheel position on audio timeupdate
    audioElement.addEventListener('loadedmetadata', () => {
        // when track metadata is loaded, update jogwheel reference
        if (id === "1") {
            jogwheel1.setAudioElement(audio.element1);
        } else {
            jogwheel2.setAudioElement(audio.element2);
        }
        
        // pdate button states
        updateButtonState(id);
    });
}

export{init};
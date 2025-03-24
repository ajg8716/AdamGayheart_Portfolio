const vinylImage = new Image();
vinylImage.src = './media/images/vinylRecord.png'; // Path to the vinyl record image

class Jogwheel{
    constructor(canvasElement, audioElement){
        this.canvasElement = canvasElement;
        this.audioElement = audioElement;
        this.rotationAngle = 0;
        this.lastUpdateTime = Date.now();
        this.isPlaying = false;
        this.trackDuration = 0;

        //make sure the vinyl image is loaded
        if (!vinylImage.complete) {
            vinylImage.onload = () => this.draw();
        } else {
            this.draw();
        }

        requestAnimationFrame(() => this.animate());
    }

    draw() {
        if (!this.canvasElement || !vinylImage.complete) return;
    
        const ctx = this.canvasElement.getContext('2d');
        
        const centerX = this.canvasElement.width / 2;
        const centerY = this.canvasElement.height / 2;
        const radius = this.canvasElement.width / 2;
    
        // draw jogwheel with rotation
        ctx.clearRect(0, 0, this.canvasElement.width, this.canvasElement.height);
    
        // rotate canvas
        ctx.save(); // save current context state
        ctx.translate(centerX, centerY); // move origin to center of the canvas
        ctx.rotate(this.rotationAngle); // apply rotation
        ctx.translate(-centerX, -centerY); // move back origin
    
        // draw Vinyl Base
        ctx.drawImage(vinylImage, 0, 0, this.canvasElement.width, this.canvasElement.height);
    
        // draw Album Art Placeholder (Dynamic Image)
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius * 0.4, 0, Math.PI * 2);
        ctx.fillStyle = 'white'; // placeholder color for album art
        ctx.fill();
        ctx.closePath();
    
        //draw rotation position indicator line
        ctx.beginPath();
        ctx.moveTo(centerX, centerY);
        ctx.lineTo(centerX, centerY - radius * 0.8);
        ctx.strokeStyle = '#FF0000';
        ctx.lineWidth = 3;
        ctx.stroke();
        ctx.closePath();
    
        //draw dot at end of indicator line
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius * 0.05, 0, Math.PI * 2);
        ctx.fillStyle = '#FF0000';
        ctx.fill();
        ctx.closePath();
    
        //draw Spindle Peg
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius * 0.05, 0, Math.PI * 2);
        ctx.fillStyle = 'black';
        ctx.shadowColor = 'rgba(0, 0, 0, 0.5)';
        ctx.shadowBlur = 5;
        ctx.fill();
        ctx.closePath();
    
        ctx.restore(); // Restore the context state
    }
    
    animate() {
        const now = Date.now();
        const deltaTime = (now - this.lastUpdateTime) / 1000; // time in seconds
        
        //update track duration when available
        if(this.audioElement && this.audioElement.duration && !isNaN(this.audioElement.duration)) {
            this.trackDuration = this.audioElement.duration;
        }
        // if playing rotate at constant speed (clockwise)
        if(this.isPlaying && this.audioElement) {
            //calculate rotation based on track position
            //for standard track, one full rotation 1.8 seconds
            const rotationSpeed = 2 * Math.PI / 1.8; // one rotation every 1.8 seconds

            this.rotationAngle += rotationSpeed * deltaTime;
            this.rotationAngle %= (2 * Math.PI); // keep angle within 0 - 2 PI range
        }
        else if (this.audioElement && this.trackDuration > 0) {
            //if not playing but track is loaded, set the position based on currentTime
            //map the track position (0 to duration) to rotaion angle (0 to 2 pi)
            //but make it so one full rotation is 1.8 seconds of audio
            const rotationRate = 2 *  Math.PI / 1.8; // radians per second
            this.rotationAngle = (this.audioElement.currentTime * rotationRate) % (2 * Math.PI);
        }

        this.lastUpdateTime = now;
        
        //draw the updated jogwheel
        this.draw();

        //continue animation loop
        requestAnimationFrame(() => this.animate());
    }
    
    //function to start the animation when the song starts playing
    startRotation() {
        this.isPlaying = true;
        this.lastUpdateTime = Date.now();
    }
    
    //function to stop the animation when the song is paused
    stopRotation() {
        this.isPlaying = false;
    }
    
    setAudioElement(audioElement){
        this.audioElement = audioElement;
        this.rotationAngle = 0; // ensure vinyl is reset

        //Reset track duration when changing audio
        if (audioElement && audioElement.duration && !isNaN(audioElement.duration)) {
            this.trackDuration = audioElement.duration;
        } 
        else {
            this.trackDuration = 0;
        }
    }

    hasTrack() {
        return this.audioElement && this.audioElement.src && this.audioElement.src !== '';
    }
}

export {Jogwheel};  


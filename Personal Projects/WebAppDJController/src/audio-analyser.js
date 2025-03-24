//access the library we imported
const jsmediatags = window.jsmediatags;

// handle metadata extraction for track 
document.querySelector("#input1").addEventListener("change", (e) => {
    const file = e.target.files[0];
    if (!file) return;

    jsmediatags.read(file, {
        onSuccess: function(tag) {
            // handle album art
            if (tag.tags.picture) {
                const data = tag.tags.picture.data;
                const format = tag.tags.picture.format;

                let base64String = "";
                for(let i = 0; i < data.length; i++) {
                    base64String += String.fromCharCode(data[i]);
                }

                document.querySelector("#cover1").style.backgroundImage = 
                    `url(data:${format};base64,${window.btoa(base64String)})`;
            }

            // handle title and artist
            if (tag.tags.title) {
                document.querySelector("#title1").textContent = tag.tags.title;
            }
            
            if (tag.tags.artist) {
                document.querySelector("#artist1").textContent = tag.tags.artist;
            }
        },
        onError: function(error) {
            console.log("Error reading tags:", error);
        }
    });
});
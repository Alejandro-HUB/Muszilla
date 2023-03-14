import React, { useState } from 'react';
import '../css/App.css';

const Widget = ({ song, onPlay }) => {
  
    const [isPlaying, setIsPlaying] = useState(false);

    return (
        <div
            className="widget"
            style={{ backgroundImage: `url(${song.image})`, backgroundRepeat: 'no-repeat' }}
        >
            <div className="widget-overlay">
                <div className="song-title">{song.title}</div>
                <div className="artist-name">{song.artist}</div>
                <button onClick={() => onPlay(song)}>
                    {isPlaying ? "Pause" : "Play"}</button>
            </div>
        </div>
    );
};

export default Widget;
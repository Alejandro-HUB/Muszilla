import React, { useState } from "react";

const MusicPlayer = ({ title, artist, image, audio, playPreviousSong, playNextSong}) => {
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const audioRef = React.createRef();

    const handlePlayPauseClick = () => {
        if (!isPlaying) {
            audioRef.current.play();
        } else {
            audioRef.current.pause();
        }
        setIsPlaying(!isPlaying);
    };

    const handleTimeUpdate = () => {
        setCurrentTime(audioRef.current.currentTime);
    };

    const handleLoadedData = () => {
        setDuration(audioRef.current.duration);
    };

    const handleProgressClick = (event) => {
        const clickPosition = event.clientX;
        const progressbar = event.target;
        const progressBarPosition =
            progressbar.getBoundingClientRect().left + window.pageXOffset;
        const progressbarWidth = progressbar.offsetWidth;
        const progressInPercentages =
            (clickPosition - progressBarPosition) / progressbarWidth;
        audioRef.current.currentTime = progressInPercentages * audioRef.current.duration;
    };

    const formatTime = (timeInSeconds) => {
        const minutes = Math.floor(timeInSeconds / 60);
        const seconds = Math.floor(timeInSeconds % 60);
        const formattedSeconds = seconds < 10 ? `0${seconds}` : seconds;
        return `${minutes}:${formattedSeconds}`;
    };

    return (
        <div className="music-player">
            <div className="music-player-details">
                <img src={image} alt="Album Art" />
                <div>
                    <h2>{title}</h2>
                    <h3>{artist}</h3>
                </div>
            </div>
            <div className="music-player-controls">
                <button onClick={handlePlayPauseClick}>
                    {isPlaying ? "Pause" : "Play"}
                </button>
                <div className="progress-bar" onClick={handleProgressClick}>
                    <div
                        className="progress-bar-filled"
                        style={{ width: `${(currentTime / duration) * 100}%` }}
                    ></div>
                </div>
                <span className="time">{formatTime(currentTime)}</span>
                <span className="time">{formatTime(duration)}</span>
                {/*<button onClick={playPreviousSong}>Previous</button>
                <button onClick={playNextSong}>Next</button>*/}
            </div>
            <audio
                ref={audioRef}
                src={audio}
                onTimeUpdate={handleTimeUpdate}
                onLoadedData={handleLoadedData}
            />
        </div>
    );
};

export default MusicPlayer;
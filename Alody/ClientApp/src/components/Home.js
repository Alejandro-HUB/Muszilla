import React, { Component, useState } from 'react';
import NavigationBar from "./NavigationBar";
import Widget from "./Widget";
import MusicPlayer from "./MusicPlayer";
import '../css/App.css';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (<App />);
    }
}


const App = () => {
    const [widgets, setWidgets] = useState([
        {
            id: 1,
            title: 'Most Played',
            songs: [
                {
                    id: 1,
                    title: 'Song 1',
                    artist: 'Artist 1',
                    image: 'https://via.placeholder.com/150',
                    audio: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3'
                }
            ]
        },
        {
            id: 2,
            title: 'Top Rated',
            songs: [
                {
                    id: 2,
                    title: 'Song 2',
                    artist: 'Artist 2',
                    image: 'https://via.placeholder.com/150',
                    audio: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3'
                }
            ]
        },
        {
            id: 3,
            title: 'Featured',
            songs: [
                {
                    id: 3,
                    title: 'Song 3',
                    artist: 'Artist 3',
                    image: 'https://via.placeholder.com/150',
                    audio: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3'
                }
            ]
        }
    ]);
    const [currentSong, setCurrentSong] = useState(widgets[0].songs[0]);
    const [currentIndex, setCurrentIndex] = useState(0);

    const handlePlay = (song) => {
        setCurrentSong(song);
    };

    const playNextSong = () => {
        const currentWidget = widgets[currentIndex];
        const nextIndex = (currentIndex + 1) % currentWidget.songs.length;
        const nextSong = currentWidget.songs[nextIndex];
        setCurrentSong(nextSong);
        setCurrentIndex(nextIndex);
    };

    const playPreviousSong = () => {
        const currentWidget = widgets[currentIndex];
        const previousIndex = (currentIndex - 1 + currentWidget.songs.length) % currentWidget.songs.length;
        const previousSong = currentWidget.songs[previousIndex];
        setCurrentSong(previousSong);
        setCurrentIndex(previousIndex);
    };

    const addWidget = () => {
        const newWidget = {
            id: Date.now(),
            title: 'New Widget',
            songs: [
                {
                    id: Date.now(),
                    title: 'Song 4',
                    artist: 'Artist 4',
                    image: 'https://via.placeholder.com/150',
                    audio: 'https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3'
                }
            ]
        };
        setWidgets([...widgets, newWidget]);
    };

    const removeWidget = (widgetId) => {
        setWidgets(widgets.filter((widget) => widget.id !== widgetId));
    };

    return (
        <div className="">
            <div className="widget-container">
                {widgets.map((widget) => (
                    <div className="widget-category" key={widget.id}>
                        <h2>{widget.title}</h2>
                        {widget.songs.map((song) => (
                            <Widget key={song.id} song={song} onPlay={handlePlay} />
                        ))}
                        <button onClick={() => removeWidget(widget.id)}>Remove Widget</button>
                    </div>
                ))}
            </div>
            <button onClick={addWidget}>Add Widget</button>
            <MusicPlayer
                title={currentSong.title}
                artist={currentSong.artist}
                image={currentSong.image}
                audio={currentSong.audio}
                playNextSong={playNextSong}
                playPreviousSong={playPreviousSong}
            />
        </div>
    );
};

export default App;
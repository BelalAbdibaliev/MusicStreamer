"use client";

import { useState, useEffect, useRef } from "react";
import { Search, Play, Pause, SkipBack, SkipForward } from "lucide-react";

import "./App.css";

interface Track {
  key: string;
  title: string;
  artist: string;
  album: string;
  url: string;
}

function App() {
  const [tracks, setTracks] = useState<Track[]>([]);
  const [filteredTracks, setFilteredTracks] = useState<Track[]>([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [currentTrack, setCurrentTrack] = useState<Track | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const observerRef = useRef<HTMLDivElement>(null);

  const audioRef = useRef<HTMLAudioElement>(null);

  const API_URL = "http://13.62.1.64:8080/api/File";

  const fetchTracks = async (pageNumber = 1, pageSize = 10) => {
    try {
      setLoading(true);
      const response = await fetch(
        `${API_URL}?page=${pageNumber}&pageSize=${pageSize}`
      );
      const data = await response.json();

      const tracksWithUrl = data.map((track: any) => ({
        ...track,
        url: `http://13.62.1.64:8080/api/file/stream/${track.key}`,
      }));

      if (pageNumber === 1) {
        setTracks(tracksWithUrl);
        setFilteredTracks(tracksWithUrl);
      } else {
        setTracks((prev) => [...prev, ...tracksWithUrl]);
        setFilteredTracks((prev) => [...prev, ...tracksWithUrl]);
      }

      // Если меньше pageSize, значит достигли конца
      if (data.length < pageSize) {
        setHasMore(false);
      }

      console.log("Fetched", data);
    } catch (error) {
      console.error("Error fetching tracks:", error);
    } finally {
      setLoading(false);
    }
  };

  // HTTP PATCH запрос для управления воспроизведением
  /*const updatePlaybackStatus = async (trackId: string, action: "play" | "pause" | "stop") => {
    try {
      // Здесь должен быть запрос к вашему серверу
      // const response = await fetch(`YOUR_SERVER_URL/tracks/${trackId}/playback`, {
      //   method: 'PATCH',
      //   headers: {
      //     'Content-Type': 'application/json',
      //   },
      //   body: JSON.stringify({
      //     action,
      //     timestamp: Date.now(),
      //     position: currentTime,
      //   }),
      // })

      // Логирование для демонстрации
      console.log(`Track ${trackId}: ${action} at position ${currentTime}s`)
    } catch (error) {
      console.error("Error updating playback status:", error)
    }
  }*/

  // Поиск треков
  const handleSearch = (query: string) => {
    setSearchQuery(query);
    if (!query.trim()) {
      setFilteredTracks(tracks);
    } else {
      const filtered = tracks.filter(
        (track) =>
          track.title.toLowerCase().includes(query.toLowerCase()) ||
          track.artist.toLowerCase().includes(query.toLowerCase()) ||
          track.album.toLowerCase().includes(query.toLowerCase())
      );
      setFilteredTracks(filtered);
    }
  };

  // Воспроизведение трека
  const playTrack = (track: Track) => {
    if (currentTrack?.key === track.key && isPlaying) {
      audioRef.current?.pause();
      setIsPlaying(false);
    } else if (currentTrack?.key === track.key && !isPlaying) {
      audioRef.current?.play();
      setIsPlaying(true);
    } else {
      setCurrentTrack(track);
      setIsPlaying(true);
    }
  };

  // Форматирование времени
  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  // Обновление времени воспроизведения
  const handleTimeUpdate = () => {
    if (audioRef.current) {
      setCurrentTime(audioRef.current.currentTime);
    }
  };

  // Обновление длительности
  const handleLoadedMetadata = () => {
    if (audioRef.current) {
      setDuration(audioRef.current.duration);
    }
  };

  // Следующий трек при окончании
  const playNext = () => {
    if (!currentTrack) return;
    const currentIndex = filteredTracks.findIndex(
      (track) => track.key === currentTrack.key
    );
    const nextIndex = (currentIndex + 1) % filteredTracks.length;
    playTrack(filteredTracks[nextIndex]);
  };

  const playPrevious = () => {
    if (!currentTrack) return;
    const currentIndex = filteredTracks.findIndex(
      (track) => track.key === currentTrack.key
    );
    const prevIndex =
      (currentIndex - 1 + filteredTracks.length) % filteredTracks.length;
    playTrack(filteredTracks[prevIndex]);
  };

  useEffect(() => {
    fetchTracks();
  }, []);

  useEffect(() => {
    const audio = audioRef.current;
    if (audio && currentTrack) {
      audio.src = currentTrack.url;
      if (isPlaying) {
        audio.play().catch(console.error);
      }
    }
  }, [currentTrack]);

  useEffect(() => {
    const audio = audioRef.current;
    if (audio) {
      if (isPlaying) {
        audio.play().catch(console.error);
      } else {
        audio.pause();
      }
    }
  }, [isPlaying]);

  useEffect(() => {
    if (loading || !hasMore) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          setPage((prev) => prev + 1);
        }
      },
      {
        root: null,
        rootMargin: "0px",
        threshold: 1.0,
      }
    );

    const node = observerRef.current;
    if (node) observer.observe(node);

    return () => {
      if (node) observer.unobserve(node);
    };
  }, [loading, hasMore]);

  useEffect(() => {
    fetchTracks(page);
  }, [page]);

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-text">Загрузка треков...</div>
      </div>
    );
  }

  return (
    <div className="app">
      <div className="container">
        {/* Заголовок и поиск */}
        <div className="header">
          <h1 className="title">Музыкальный плеер</h1>
          <div className="search-container">
            <Search className="search-icon" size={16} />
            <input
              type="text"
              placeholder="Поиск треков, исполнителей, альбомов..."
              value={searchQuery}
              onChange={(e) => handleSearch(e.target.value)}
              className="search-input"
            />
          </div>
        </div>

        {/* Список треков */}
        <div className="tracks-list">
          {filteredTracks.map((track) => (
            <div key={track.key} className="track-card">
              <img
                src="/placeholder.svg"
                alt={track.album}
                className="track-cover"
              />
              <div className="track-info">
                <h3 className="track-title">{track.title}</h3>
                <p className="track-artist">{track.artist}</p>
                <p className="track-album">{track.album}</p>
              </div>
              <div className="track-duration">
                {currentTrack?.key === track.key ? formatTime(currentTime) : ""}
              </div>
              <button onClick={() => playTrack(track)} className="play-button">
                {currentTrack?.key === track.key && isPlaying ? (
                  <Pause size={16} />
                ) : (
                  <Play size={16} />
                )}
              </button>
            </div>
          ))}
        </div>

        {/* Наблюдаемый элемент для пагинации */}
        <div ref={observerRef} style={{ height: 20 }} />

        {loading && <div className="loading-text">Загрузка...</div>}

        {/* Минималистичный плеер */}
        {currentTrack && (
          <div className="player">
            <div className="player-info">
              <h4 className="player-title">{currentTrack.title}</h4>
              <p className="player-artist">{currentTrack.artist}</p>
            </div>

            <div className="player-progress">
              <div className="progress-time">
                <span className="current-time">{formatTime(currentTime)}</span>
                <span className="total-time">{formatTime(duration)}</span>
              </div>
              <div
                className="progress-bar"
                onClick={(e) => {
                  const rect = e.currentTarget.getBoundingClientRect();
                  const percent = (e.clientX - rect.left) / rect.width;
                  const newTime = percent * duration;
                  if (audioRef.current) {
                    audioRef.current.currentTime = newTime;
                  }
                }}
              >
                <div
                  className="progress-fill"
                  style={{
                    width: `${
                      duration > 0 ? (currentTime / duration) * 100 : 0
                    }%`,
                  }}
                />
              </div>
            </div>

            <div className="player-controls">
              <button onClick={playPrevious} className="player-button">
                <SkipBack size={20} />
              </button>

              <button
                onClick={() => playTrack(currentTrack!)}
                className="player-button"
              >
                {isPlaying ? <Pause size={20} /> : <Play size={20} />}
              </button>

              <button onClick={playNext} className="player-button">
                <SkipForward size={20} />
              </button>
            </div>
          </div>
        )}

        {/* Скрытый аудио элемент */}
        <audio
          ref={audioRef}
          onTimeUpdate={handleTimeUpdate}
          onLoadedMetadata={handleLoadedMetadata}
          onEnded={playNext}
        />
      </div>
    </div>
  );
}

export default App;

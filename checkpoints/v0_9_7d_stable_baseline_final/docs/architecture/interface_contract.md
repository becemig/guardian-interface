# Guardian Interface Core API Contract v0.9.2b-stable

## ReplayEngine
- load_replay_as_snapshots(path)

## ReplayController
- load_snapshots(snapshots)
- play()
- pause()
- step_forward()
- step_backward()
- seek_to_frame(index)
- set_playback_speed(speed)

Signals:
- replay_frame(snapshot)
- index_changed(frame_index, total_frames)
- playback_state_changed(is_playing)

## TelemetryModel
- ingest_replay_snapshot(snapshot)
- get_dashboard_summary()

Signals:
- dashboard_data_updated(summary)

## ResearchEventSystem
- add_event(frame_index, event_type, label, notes)
- get_events_for_frame(frame_index)
- handle_replay_index_changed(frame_index, total_frames)

Signals:
- events_matched(frame_index, events)

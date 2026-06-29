#!/usr/bin/env node
/**
 * Retro Football '76 — quick console demo (no Unity required)
 * Usage: node tools/run-demo.js
 */
const fs = require('fs');
const path = require('path');

const teamsPath = path.join(__dirname, '..', 'Assets', 'StreamingAssets', 'Historical', 'teams.json');
const file = JSON.parse(fs.readFileSync(teamsPath, 'utf8'));
const home = file.teams.find(t => t.id === 'ned1974');
const away = file.teams.find(t => t.id === 'frg1974');

const PHASES_PER_HALF = 12;
const SQUAD = 5;

function power(team, sw, pw, dw) {
  const squad = team.players.slice(0, SQUAD);
  if (!squad.length) return 50;
  return squad.reduce((s, p) => s + p.shoot * sw + p.pass * pw + p.defend * dw + p.pace * 0.1, 0) / squad.length;
}

function pick(team, preferShoot) {
  return [...team.players].slice(0, SQUAD).sort((a, b) => (preferShoot ? b.shoot - a.shoot : b.pass - a.pass))[0];
}

function simulate(home, away, seed = 76) {
  let rng = seed;
  const rand = () => { rng = (rng * 1103515245 + 12345) & 0x7fffffff; return rng / 0x7fffffff; };
  const events = [];
  let minute = 0, homeScore = 0, awayScore = 0;

  const add = (min, desc) => events.push({ minute: min, description: desc });

  add(0, `${home.name} vs ${away.name} — kickoff!`);

  for (let phase = 0; phase < PHASES_PER_HALF * 2; phase++) {
    minute = Math.min(90, Math.floor(((phase + 1) / (PHASES_PER_HALF * 2)) * 90));
    if (phase === PHASES_PER_HALF) add(minute, 'Half time.');

    const attack = rand() > 0.5 ? home : away;
    const defend = attack.id === home.id ? away : home;
    const atk = power(attack, 0.45, 0.35, 0.2);
    const def = power(defend, 0.15, 0.25, 0.6);
    const roll = rand() * (atk + def);
    const shooter = pick(attack, true);

    if (roll < atk * 0.12) {
      if (attack.id === home.id) homeScore++; else awayScore++;
      add(minute, `GOAL! ${shooter.name} (${attack.name}) ${homeScore}-${awayScore}`);
    } else if (roll < atk * 0.35) {
      add(minute, `${shooter.name} shoots — saved!`);
    } else if (roll < atk * 0.55) {
      add(minute, `${defend.name} win the ball.`);
    } else {
      add(minute, `${shooter.name} — shot wide.`);
    }
  }

  add(90, `Full time: ${home.name} ${homeScore} - ${awayScore} ${away.name}`);
  return { events, homeScore, awayScore };
}

console.log("Retro Football '76 — Console Demo\n");
console.log(`Match: ${home.name} (1974) vs ${away.name} (1974)`);
console.log('-'.repeat(50));

const result = simulate(home, away);
for (const e of result.events) {
  console.log(`[${String(e.minute).padStart(2)}'] ${e.description}`);
}
console.log('-'.repeat(50));
console.log(`Final: ${home.name} ${result.homeScore} - ${result.awayScore} ${away.name}`);
console.log('\nFor full game UI: install Unity Hub 2022.3 LTS → open this project → Generate MVP Scenes → Play');

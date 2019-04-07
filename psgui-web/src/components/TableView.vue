<template>
	<div style="width: 100%;height: 100%;flex-direction: column;" class="d-flex" >
		<div>
			<input type="text" data-role="input" v-model="command" @blur="enteredCommand()">
		</div>
		<div v-if="data">
			<!-- <select @change="performAction($event)">
				<option v-for="action in setActions()" :key="action" value="action" >{{action}}</option>
			</select> -->
			<span v-for="tracker in trackers" :key="tracker.index" class="clickable" @click="trackTo(tracker)">
				{{tracker.name}}
			</span>
		</div>
		<div class="d-flex" style="flex-direction: row; flex-grow=1">
			<div style="overflow: auto;">
				<div class="table-view-container" v-if="data && data.Metadata">
					<table class="table striped table-border mt-4">
						<thead>
							<tr>
								<th>
									Id
								</th>
								<th v-for="column in data.Metadata.Columns" :key="column">
									{{ column }}
								</th>
							</tr>
						</thead>
						<tbody>
							<tr v-for="obj in data.ProjectedObjects" :key="obj.Id">
								<template v-if="obj.Value">
									{{obj.Value}}
								</template>
								<template v-else-if="!obj.IsNull">
									<td>
										<input type="checkbox" data-role="checkbox" :data-caption="obj.Id" data-caption-position="right"
											v-model="obj.checked">
									</td>
									<td v-for="prop in getColumnValues(obj)" :key="prop.Name">
										<template v-if="prop.IsPrimitive">
											{{prop.Value}}
										</template>
										<template v-else-if="prop.IsNotContained">
											(null)
										</template>
										<template v-else>
											<span class="clickable" @click="openTableFor(obj.Id, prop.Name)">...</span>
										</template>
									</td>
								</template>
								<template v-else>
									(null)
								</template>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
			<div>
				<ul class="custom-list-marker">
					<li v-for="action in setActions()" :key="action" value="action" @click="performAction(action)" class="clickable">
						{{action}}
					</li>
				</ul>
			</div>
		</div>
	</div>
</template>

<script>

export default {
	name: 'TableView',
	props: {
		msg: String
	},
	data() {
		return {
			data: {},
			command: "ls",
			trackers: [],
			actions: []
		}
	},
	methods: {
		checkboxClicked(obj) {
			this.setActions();
		},
		enteredCommand() {
			this.trackers = [{
				index: 0,
				name: 'root',
				command: this.command
			}];
			this.getData();
		},
		performAction(action) {
			let objects = this.data.ProjectedObjects.filter(o => o.checked);
			for(let i = 0; i < objects.length; i++) {
				this.$http.post('https://localhost:44398/api/command/', 
					{Command: this.command + ` | select -index ${objects[i].Id} | % ${action}`}).then(response => {
						this.data = response.body;
					});
			}
		},
		setActions() {
			if(!this.data.ProjectedObjects) return;
			let objects = this.data.ProjectedObjects.filter(o => o.checked);
			let actions = [];
			for(let i = 0; i < objects.length; i++) {
				for(let j = 0; j < objects[i].Actions.length; j++)
				if(!actions.includes(objects[i].Actions[j].Name)) {
					actions.push(objects[i].Actions[j].Name);
				}
			}
			return actions;
		},
		trackTo(tracker) {
			this.trackers = this.trackers.slice(0, tracker.index + 1);
			this.command = tracker.command;
			this.getData();
		},
		getData() {
			this.$http.post('https://localhost:44398/api/command/', {Command: this.command}).then(response => {
				this.data = response.body;
			}, response => {
				let a = 1;
			});
		},
		openTableFor(id, name) {
			this.command = `${this.command} | select -index ${id} | select -expand ${name}`;
			this.trackers.push({
				index: this.trackers.length,
				name: `[${id}].${name}`,
				command: this.command
			});
			this.getData();
		},
		getColumnValues(obj) {
			let values = []
			for(let i = 0; i < this.data.Metadata.Columns.length; i++) {
				let columnName = this.data.Metadata.Columns[i];
				let found = obj.Properties.filter(p => p.Name === columnName);
				if(!found || found.length < 1) {
					values.push({Name: columnName, IsNotContained: true});
				} else {
					values.push(found[0]);
				}
			}
			return values;
		}
	}
}
</script>

<style scoped>
	.content-view {
		display: flex;
	}
	.clickable {
		cursor: pointer;
	}
	.table-view-container .table th,
	.table-view-container .table tbody td {
		padding: 0rem .5rem;
	}
</style>
